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

    public CallbackController(
        IDepositWithdrawRequestRepository requestRepository,
        ITransactionRepository transactionRepository,
        IWalletRepository walletRepository,
        IConfiguration configuration,
        IOptionsMonitor<List<ClientSettings>> optionsMonitor)
    {
        _requestRepository = requestRepository;
        _transactionRepository = transactionRepository;
        _walletRepository = walletRepository;
        _configuration = configuration;
        _clients = optionsMonitor.CurrentValue;
    }

    // Done
    [HttpPost]
    public async Task<IActionResult> HandleDeposit([FromBody] CallbackResponse response)
    {
        if (!ModelState.IsValid)
        {
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
            return BadRequest(new
            {
                ErrorCode = "INVALID_HASH",
                Status = false
            });
        }

        var requestResult = await _requestRepository.UpdateStatusAsync(response.TransactionId, response.Status);
        if (!requestResult)
        {
            return BadRequest(new
            {
                ErrorCode = "FAILED_UPDATE_DEPOSITWITHDRAW",
                Status = false
            });
        }

        var userId = await _requestRepository
            .GetUserIdAsync(response.TransactionId);
        ArgumentException.ThrowIfNullOrEmpty(userId);

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
                return BadRequest(new
                {
                    ErrorCode = "FAILED_UPDATE_WALLET",
                    Status = false
                });
            }
        }

        return Ok(new
        {
            Status = true
        });
    }

    [HttpPost]
    public async Task<IActionResult> HandleWithdraw([FromBody] CallbackResponse response)
    {
        if (!ModelState.IsValid)
        {
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
            return BadRequest(new
            {
                ErrorCode = "INVALID_HASH",
                Status = false
            });
        }

        var requestResult = await _requestRepository.UpdateStatusAsync(response.TransactionId, response.Status);
        if (!requestResult)
        {
            return BadRequest(new
            {
                ErrorCode = "FAILED_UPDATE_DEPOSITWITHDRAW",
                Status = false
            });
        }

        var userId = await _requestRepository
            .GetUserIdAsync(response.TransactionId);
        ArgumentException.ThrowIfNullOrEmpty(userId);

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
            return BadRequest(new
            {
                ErrorCode = "FAILED_CREATE_TRANSACTION",
                Status = false
            });
        }
        if (response.Status == "Success")
        {
            var walletResult = await _walletRepository.WithdrawBalanceAsync(userId, response.Amount);
            if (!walletResult)
            {
                return BadRequest(new
                {
                    ErrorCode = "FAILED_UPDATE_WALLET",
                    Status = false
                });
            }
        }

        return Ok(new
        {
            Status = true
        });
    }
}
