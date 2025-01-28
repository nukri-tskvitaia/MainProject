using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MvcProject.Data.Repositories;
using MvcProject.Services;

namespace MvcProject.Controllers;

[Authorize(Roles = "Player")]
public class TransactionController : BaseController
{
    private readonly IDepositWithdrawService _depositWithdrawService;
    private readonly IBankingApiService _bankingApiService;
    private readonly ITransactionRepository _transactionRepository;
    private readonly ILogger<TransactionController> _logger;

    public TransactionController(
        IBankingApiService bankingApiService,
        IDepositWithdrawService depositWithdrawService,
        ITransactionRepository transactionRepository,
        ILogger<TransactionController> logger)
    {
        _bankingApiService = bankingApiService;
        _depositWithdrawService = depositWithdrawService;
        _transactionRepository = transactionRepository;
        _logger = logger;
    }

    public IActionResult Deposit() => View();

    // Done
    [HttpPost]
    public async Task<IActionResult> Deposit(decimal amount)
    {
        _logger.LogInformation("Deposit method called with amount: {Amount}", amount);

        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Model state is invalid.");
            return BadRequest(new { Message = ModelState });
        }

        var userId = GetUserId();
        if (userId == null)
        {
            _logger.LogWarning("User is not authenticated.");
            return Unauthorized(new { Message = "User is not authenticated." });
        }

        _logger.LogInformation("Adding deposit for user ID: {UserId}", userId);
        var depositWithdrawId = await _depositWithdrawService.AddDepositAsync(
            userId, amount);
        if (depositWithdrawId is null)
        {
            _logger.LogError("Failed to add deposit for user ID: {UserId}", userId);
            return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Internal server error." });
        }

        _logger.LogInformation("Calling banking API for deposit ID: {DepositWithdrawId}", depositWithdrawId);
        var bankingApiResponse = await _bankingApiService.DepositBankingApiAsync(
            depositWithdrawId, amount);
        if (bankingApiResponse is null)
        {
            _logger.LogError("Banking API call failed for deposit ID: {DepositWithdrawId}", depositWithdrawId);
            return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Internal server error." });
        }

        if (bankingApiResponse.Status == "Success" &&
            !string.IsNullOrWhiteSpace(bankingApiResponse.PaymentUrl))
        {
            _logger.LogInformation("Payment processed successfully for deposit ID: {DepositWithdrawId}", depositWithdrawId);
            return Ok(new { Status = "Success", bankingApiResponse.PaymentUrl });
        }

        var errorMessage = bankingApiResponse.ErrorCode switch
        {
            "INVALID_DATA" => "The data provided for the deposit is invalid.",
            "INVALID_HASH" => "The request failed hash validation. Please try again.",
            _ => "An unknown error occurred. Please try again later."
        };

        _logger.LogWarning("Payment failed for deposit ID: {DepositWithdrawId} with error: {ErrorMessage}", depositWithdrawId, errorMessage);
        return BadRequest(new { Message = errorMessage });
    }

    public IActionResult Withdraw() => View();

    [HttpPost]
    public async Task<IActionResult> Withdraw(decimal amount)
    {
        _logger.LogInformation("Withdraw method called with amount: {Amount}", amount);

        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Model state is invalid.");
            return BadRequest(new { Message = ModelState });
        }

        var userId = GetUserId();
        if (userId == null)
        {
            _logger.LogWarning("User is not authenticated.");
            return Unauthorized(new { Message = "User is not authenticated." });
        }

        _logger.LogInformation("Adding withdraw request for user ID: {UserId}", userId);
        var depositWithdrawId = await _depositWithdrawService.AddWithdrawAsync(
    userId, amount);
        if (depositWithdrawId is null)
        {
            _logger.LogError("Failed to add withdraw request for user ID: {UserId}", userId);
            return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Internal server error." });
        }

        _logger.LogInformation("Withdraw request processed successfully for user ID: {UserId}", userId);
        return Ok(new { Status = "Success", Message = "Your request will be considered by admin." });
    }

    public IActionResult TransactionHistory() => View();

    [HttpGet]
    public async Task<IActionResult> GetTransactionHistory()
    {
        var userId = GetUserId();
        _logger.LogInformation("GetTransactionHistory method called for user ID: {UserId}", userId);
        
        if (userId == null)
        {
            _logger.LogWarning("User is not authenticated.");
            return Unauthorized(new { Message = "User is not authenticated." });
        }

        _logger.LogInformation("Fetching transaction history for user ID: {UserId}", userId);
        var transactions = await _transactionRepository.GetAllUserAsync(userId);

        _logger.LogInformation("Transaction history retrieved successfully for user ID: {UserId}", userId);
        return Json(new { data = transactions });
    }
}