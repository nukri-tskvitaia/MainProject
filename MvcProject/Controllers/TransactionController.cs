using log4net;
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
    private static readonly ILog _logger = LogManager.GetLogger(typeof(TransactionController));

    public TransactionController(
        IBankingApiService bankingApiService,
        IDepositWithdrawService depositWithdrawService,
        ITransactionRepository transactionRepository)
    {
        _bankingApiService = bankingApiService;
        _depositWithdrawService = depositWithdrawService;
        _transactionRepository = transactionRepository;
    }

    public IActionResult Deposit() => View();

    // Done
    [HttpPost]
    public async Task<IActionResult> Deposit(decimal amount)
    {
        _logger.InfoFormat("Deposit method called with amount: {0}", amount);

        if (!ModelState.IsValid)
        {
            _logger.Warn("Model state is invalid.");
            return BadRequest(new { Message = ModelState });
        }

        var userId = GetUserId();
        if (userId == null)
        {
            _logger.Warn("User is not authenticated.");
            return Unauthorized(new { Message = "User is not authenticated." });
        }

        _logger.InfoFormat("Adding deposit for user ID: {0}", userId);
        var depositWithdrawResponse = await _depositWithdrawService.AddDepositAsync(
            userId, amount);
        if (depositWithdrawResponse.Status == "Failed" || depositWithdrawResponse.DepositWithdrawId == null)
        {
            _logger.ErrorFormat("Failed to add deposit. {0}", depositWithdrawResponse.ErrorMessage);
            return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Internal server error." });
        }

        _logger.InfoFormat("Calling banking API for deposit ID: {0}", depositWithdrawResponse.DepositWithdrawId);
        var bankingApiResponse = await _bankingApiService.DepositBankingApiAsync(
            (int)depositWithdrawResponse.DepositWithdrawId, amount);
        if (bankingApiResponse is null)
        {
            _logger.ErrorFormat("Banking API call failed for deposit ID: {0}", depositWithdrawResponse.DepositWithdrawId);
            return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Internal server error." });
        }

        if (bankingApiResponse.Status == "Success" &&
            !string.IsNullOrWhiteSpace(bankingApiResponse.PaymentUrl))
        {
            _logger.InfoFormat("Payment processed successfully for deposit ID: {0}", depositWithdrawResponse.DepositWithdrawId);
            return Ok(new { Status = "Success", bankingApiResponse.PaymentUrl });
        }

        var errorMessage = bankingApiResponse.ErrorCode switch
        {
            "INVALID_DATA" => "The data provided for the deposit is invalid.",
            "INVALID_HASH" => "The request failed hash validation. Please try again.",
            _ => "An unknown error occurred. Please try again later."
        };

        _logger.WarnFormat("Payment failed for deposit ID: {0} with error: {1}", depositWithdrawResponse.DepositWithdrawId, errorMessage);
        return BadRequest(new { Message = errorMessage });
    }

    public IActionResult Withdraw() => View();

    [HttpPost]
    public async Task<IActionResult> Withdraw(decimal amount)
    {
        _logger.InfoFormat("Withdraw method called with amount: {0}", amount);

        if (!ModelState.IsValid)
        {
            _logger.Warn("Model state is invalid.");
            return BadRequest(new { Message = ModelState });
        }

        var userId = GetUserId();
        if (userId == null)
        {
            _logger.Warn("User is not authenticated.");
            return Unauthorized(new { Message = "User is not authenticated." });
        }

        _logger.InfoFormat("Adding withdraw request for user ID: {0}", userId);
        var depositWithdrawId = await _depositWithdrawService.AddWithdrawAsync(
    userId, amount);
        if (depositWithdrawId is null)
        {
            _logger.ErrorFormat("Failed to add withdraw request for user ID: {0}", userId);
            return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Internal server error." });
        }

        _logger.InfoFormat("Withdraw request processed successfully for user ID: {0}", userId);
        return Ok(new { Status = "Success", Message = "Your request will be considered by admin." });
    }

    public IActionResult TransactionHistory() => View();

    [HttpGet]
    public async Task<IActionResult> GetTransactionHistory()
    {
        var userId = GetUserId();
        _logger.InfoFormat("GetTransactionHistory method called for user ID: {0}", userId);
        
        if (userId == null)
        {
            _logger.Warn("User is not authenticated.");
            return Unauthorized(new { Message = "User is not authenticated." });
        }

        _logger.InfoFormat("Fetching transaction history for user ID: {0}", userId);
        var transactions = await _transactionRepository.GetAllUserAsync(userId);

        _logger.InfoFormat("Transaction history retrieved successfully for user ID: {0}", userId);
        return Json(new { data = transactions });
    }
}