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
        if (!ModelState.IsValid)
        {
            return BadRequest(new { Message = ModelState });
        }

        var userId = GetUserId();
        if (userId == null)
        {
            return Unauthorized(new { Message = "User is not authenticated." });
        }

        var depositWithdrawId = await _depositWithdrawService.AddDepositAsync(
            userId, amount);
        if (depositWithdrawId is null)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Internal server error." });
        }

        var bankingApiResponse = await _bankingApiService.DepositBankingApiAsync(
            depositWithdrawId, amount);
        if (bankingApiResponse is null)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Internal server error." });
        }

        if (bankingApiResponse.Status == "Success" &&
            !string.IsNullOrWhiteSpace(bankingApiResponse.PaymentUrl))
        {
            return Ok(new { Status = "Success", bankingApiResponse.PaymentUrl });
        }

        var errorMessage = bankingApiResponse.ErrorCode switch
        {
            "INVALID_DATA" => "The data provided for the deposit is invalid.",
            "INVALID_HASH" => "The request failed hash validation. Please try again.",
            _ => "An unknown error occurred. Please try again later."
        };

        return BadRequest(new { Message = errorMessage });
    }

    public IActionResult Withdraw() => View();

    [HttpPost]
    public async Task<IActionResult> Withdraw(decimal amount)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new { Message = ModelState });
        }

        var userId = GetUserId();
        if (userId == null)
        {
            return Unauthorized(new { Message = "User is not authenticated." });
        }

        var depositWithdrawId = await _depositWithdrawService.AddWithdrawAsync(
    userId, amount);
        if (depositWithdrawId is null)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Internal server error." });
        }

        return Ok(new { Status = "Success", Message = "Your request will be considered by admin." });
    }

    public IActionResult TransactionHistory() => View();

    [HttpGet]
    public async Task<IActionResult> GetTransactionHistory()
    {
        var userId = GetUserId();
        if (userId == null)
        {
            return Unauthorized(new { Message = "User is not authenticated." });
        }

        var transactions = await _transactionRepository.GetAllUserAsync(userId);

        return Json(new { data = transactions });
    }
}