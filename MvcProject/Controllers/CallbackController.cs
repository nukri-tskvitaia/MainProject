using log4net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MvcProject.Configuration;
using MvcProject.Data.Repositories;
using MvcProject.DTO;
using MvcProject.Helper;
using MvcProject.Models;

namespace MvcProject.Controllers;

public class CallbackController : Controller
{
    private readonly IDepositWithdrawRequestRepository _requestRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IWalletRepository _walletRepository;
    private readonly IProcessRepository _processRepository;
    private readonly IConfiguration _configuration;
    private readonly List<ClientSettings> _clients;
    private static readonly ILog _logger = LogManager.GetLogger(typeof(CallbackController));

    public CallbackController(
        IDepositWithdrawRequestRepository requestRepository,
        ITransactionRepository transactionRepository,
        IWalletRepository walletRepository,
        IProcessRepository processRepository,
        IConfiguration configuration,
        IOptionsMonitor<List<ClientSettings>> optionsMonitor)
    {
        _requestRepository = requestRepository;
        _transactionRepository = transactionRepository;
        _walletRepository = walletRepository;
        _processRepository = processRepository;
        _configuration = configuration;
        _clients = optionsMonitor.CurrentValue;
    }

    // Done
    [HttpPost]
    public async Task<IActionResult> HandleDeposit([FromBody] CallbackResponse response)
    {
        _logger.InfoFormat("Handling deposit callback for TransactionId: {0}", response.TransactionId);

        if (!ModelState.IsValid)
        {
            _logger.WarnFormat("Invalid model state for deposit callback: {0}", response);
            return BadRequest(new
            {
                ErrorCode = "INVALID_DATA",
                Status = false
            });
        }

        var validationResult = RequestValidator.ValidateMerchant(
            _clients, response.ClientId);
        if (!validationResult)
        {
            _logger.WarnFormat("Invalid client ID validation failed for TransactionId: {0}", response.TransactionId);
            return BadRequest(new
            {
                ErrorCode = "INVALID_DATA",
                Status = false
            });
        }

        var hashValidationResult = HashGenerator.GenerateHash(
            response.Amount, response.ClientId,
            response.TransactionId, _configuration["Secrets:Key"]!);
        if (hashValidationResult != response.Hash)
        {
            _logger.WarnFormat("Invalid hash for TransactionId: {0}. Expected: {1}, Received: {2}", response.TransactionId, hashValidationResult, response.Hash);
            return BadRequest(new
            {
                ErrorCode = "INVALID_HASH",
                Status = false
            });
        }

        var userId = await _requestRepository
            .GetUserIdAsync(response.TransactionId);
        ArgumentException.ThrowIfNullOrEmpty(userId);

        var transaction = new Transaction
        {
            Id = Guid.NewGuid().ToString(),
            UserId = userId,
            Amount = response.Amount,
            Status = response.Status,
            CreatedAt = DateTime.UtcNow,
            TransactionType = "Deposit"
        };

        _logger.Info("Processing Deposit");
        var result = await _processRepository.ProcessDepositAsync(response.TransactionId, transaction);

        if (result.Status == "Failed")
        {
            _logger.ErrorFormat("Processing Deposit Failed - {0}", result.ErrorMessage);
            return BadRequest(new
            {
                ErrorCode = "FAILED_PROCESS_DEPOSIT",
                Status = false
            });
        }

        _logger.InfoFormat("Successfully processed deposit callback for TransactionId: {0}", response.TransactionId);
        return Ok(new
        {
            Status = true
        });
    }

    [HttpPost]
    public async Task<IActionResult> HandleWithdraw([FromBody] CallbackResponse response)
    {
        _logger.InfoFormat("Handling withdraw callback for TransactionId: {0}", response.TransactionId);

        if (!ModelState.IsValid)
        {
            _logger.WarnFormat("Invalid model state for withdraw callback: {0}", response);
            return BadRequest(new
            {
                ErrorCode = "INVALID_DATA",
                Status = false
            });
        }

        var validationResult = RequestValidator.ValidateMerchant(
            _clients, response.ClientId);
        if (!validationResult)
        {
            _logger.WarnFormat("Invalid client ID validation failed for TransactionId: {0}", response.TransactionId);
            return BadRequest(new
            {
                ErrorCode = "INVALID_DATA",
                Status = false
            });
        }

        var hashValidationResult = HashGenerator.GenerateHash(
            response.Amount, response.ClientId,
            response.TransactionId, _configuration["Secrets:Key"]!);
        if (hashValidationResult != response.Hash)
        {
            _logger.WarnFormat("Invalid hash for TransactionId: {0}. Expected: {1}, Received: {2}", response.TransactionId, validationResult, response.Hash);
            return BadRequest(new
            {
                ErrorCode = "INVALID_HASH",
                Status = false
            });
        }

        var userId = await _requestRepository
            .GetUserIdAsync(response.TransactionId);
        ArgumentException.ThrowIfNullOrEmpty(userId);

        var transaction = new Transaction
        {
            Id = Guid.NewGuid().ToString(),
            UserId = userId,
            Amount = response.Amount,
            Status = response.Status,
            CreatedAt = DateTime.UtcNow,
            TransactionType = "Withdraw"
        };

        _logger.Info("Processing Withdraw");
        var result = await _processRepository.ProcessWithdrawAsync(response.TransactionId, transaction);

        if (result.Status == "Failed")
        {
            _logger.ErrorFormat("Processing Withdraw Failed - {0}", result.ErrorMessage);
            return BadRequest(new
            {
                ErrorCode = "FAILED_PROCESS_WITHDRAW",
                Status = false
            });
        }

        _logger.InfoFormat("Successfully processed withdraw callback for TransactionId: {0}", response.TransactionId);
        return Ok(new
        {
            Status = true
        });
    }
}
