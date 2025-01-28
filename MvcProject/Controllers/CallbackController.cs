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
    private readonly IConfiguration _configuration;
    private readonly List<ClientSettings> _clients;
    private readonly ILogger<CallbackController> _logger;

    public CallbackController(
        IDepositWithdrawRequestRepository requestRepository,
        ITransactionRepository transactionRepository,
        IWalletRepository walletRepository,
        IConfiguration configuration,
        IOptionsMonitor<List<ClientSettings>> optionsMonitor,
        ILogger<CallbackController> logger)
    {
        _requestRepository = requestRepository;
        _transactionRepository = transactionRepository;
        _walletRepository = walletRepository;
        _configuration = configuration;
        _clients = optionsMonitor.CurrentValue;
        _logger = logger;
    }

    // Done
    [HttpPost]
    public async Task<IActionResult> HandleDeposit([FromBody] CallbackResponse response)
    {
        _logger.LogInformation("Handling deposit callback for TransactionId: {TransactionId}", response.TransactionId);

        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid model state for deposit callback: {@Response}", response);
            return BadRequest(new
            {
                ErrorCode = "INVALID_DATA",
                Status = false
            });
        }

        var validationResult = RequestValidator.ValidateDeposit(
            _clients, response.ClientId);
        if (!validationResult)
        {
            _logger.LogWarning("Invalid client ID validation failed for TransactionId: {TransactionId}", response.TransactionId);
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
            _logger.LogWarning("Invalid hash for TransactionId: {TransactionId}. Expected: {ExpectedHash}, Received: {ReceivedHash}", response.TransactionId, hashValidationResult, response.Hash);
            return BadRequest(new
            {
                ErrorCode = "INVALID_HASH",
                Status = false
            });
        }

        var requestResult = await _requestRepository.UpdateStatusAsync(response.TransactionId, response.Status);
        if (!requestResult)
        {
            _logger.LogError("Failed to update deposit request status for TransactionId: {TransactionId}", response.TransactionId);
            return BadRequest(new
            {
                ErrorCode = "FAILED_UPDATE_DEPOSITWITHDRAW",
                Status = false
            });
        }

        var userId = await _requestRepository
            .GetUserIdAsync(response.TransactionId);
        ArgumentException.ThrowIfNullOrEmpty(userId);

        _logger.LogInformation("Creating transaction record for TransactionId: {TransactionId}", response.TransactionId);
        var transactionResult = await _transactionRepository.CreateAsync(new Transaction
        {
            Id = Guid.NewGuid().ToString(),
            UserId = userId,
            Amount = response.Amount,
            Status = response.Status,
            CreatedAt = DateTime.UtcNow,
        });
        if (!transactionResult)
        {
            _logger.LogError("Failed to create transaction record for TransactionId: {TransactionId}", response.TransactionId);
            return BadRequest(new
            {
                ErrorCode = "FAILED_CREATE_TRANSACTION",
                Status = false
            });
        }
        if (response.Status == "Success")
        {
            var walletResult = await _walletRepository.DepositBalanceAsync(userId, response.Amount);
            if (!walletResult)
            {
                _logger.LogError("Failed to update wallet balance for UserId: {UserId}", userId);
                return BadRequest(new
                {
                    ErrorCode = "FAILED_UPDATE_WALLET",
                    Status = false
                });
            }
        }

        _logger.LogInformation("Successfully processed deposit callback for TransactionId: {TransactionId}", response.TransactionId);
        return Ok(new
        {
            Status = true
        });
    }

    [HttpPost]
    public async Task<IActionResult> HandleWithdraw([FromBody] CallbackResponse response)
    {
        _logger.LogInformation("Handling withdraw callback for TransactionId: {TransactionId}", response.TransactionId);

        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid model state for withdraw callback: {@Response}", response);
            return BadRequest(new
            {
                ErrorCode = "INVALID_DATA",
                Status = false
            });
        }

        var validationResult = RequestValidator.ValidateDeposit(
            _clients, response.ClientId);
        if (!validationResult)
        {
            _logger.LogWarning("Invalid client ID validation failed for TransactionId: {TransactionId}", response.TransactionId);
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
            _logger.LogWarning("Invalid hash for TransactionId: {TransactionId}. Expected: {ExpectedHash}, Received: {ReceivedHash}", response.TransactionId, validationResult, response.Hash);
            return BadRequest(new
            {
                ErrorCode = "INVALID_HASH",
                Status = false
            });
        }

        var userId = await _requestRepository
            .GetUserIdAsync(response.TransactionId);
        ArgumentException.ThrowIfNullOrEmpty(userId);

        var checkAvailableBalance = await _walletRepository.CheckAvailableBalanceAsync(userId);
        if ((checkAvailableBalance.CurrentBalance - checkAvailableBalance.BlockedAmount) < response.Amount)
        {
            _logger.LogWarning("Insufficient funds for UserId: {UserId}", userId);
            return BadRequest(new
            {
                ErrorCode = "INSUFFICIENT_FUNDS",
                Status = false
            });
        }

        var requestResult = await _requestRepository.UpdateStatusAsync(response.TransactionId, response.Status);
        if (!requestResult)
        {
            _logger.LogError("Failed to update withdraw request status for TransactionId: {TransactionId}", response.TransactionId);
            return BadRequest(new
            {
                ErrorCode = "FAILED_UPDATE_DEPOSITWITHDRAW",
                Status = false
            });
        }

        _logger.LogInformation("Creating transaction record for TransactionId: {TransactionId}", response.TransactionId);
        var transactionResult = await _transactionRepository.CreateAsync(new Transaction
        {
            Id = Guid.NewGuid().ToString(),
            UserId = userId,
            Amount = response.Amount,
            Status = response.Status,
            CreatedAt = DateTime.UtcNow,
        });
        if (!transactionResult)
        {
            _logger.LogError("Failed to create transaction record for TransactionId: {TransactionId}", response.TransactionId);
            return BadRequest(new
            {
                ErrorCode = "FAILED_CREATE_TRANSACTION",
                Status = false
            });
        }

        if (response.Status == "Success")
        {
            var walletResult = await _walletRepository.WithdrawBalanceAsync(userId, response.Amount, checkAvailableBalance.BlockedAmount);
            if (!walletResult)
            {
                _logger.LogError("Failed to update wallet balance for UserId: {UserId}", userId);
                return BadRequest(new
                {
                    ErrorCode = "FAILED_UPDATE_WALLET",
                    Status = false
                });
            }
        }

        _logger.LogInformation("Successfully processed withdraw callback for TransactionId: {TransactionId}", response.TransactionId);
        return Ok(new
        {
            Status = true
        });
    }
}
