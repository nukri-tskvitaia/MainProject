using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MvcProject.Models;
using MvcProject.Services;

namespace MvcProject.Controllers;

public class TransactionController : BaseController
{
    private readonly UserManager<User> _userManager;
    private readonly IDepositWithdrawService _depositWithdrawService;
    private readonly IBankingApiService _bankingApiService;

    public TransactionController(IBankingApiService bankingApiService,  UserManager<User> userManager, IDepositWithdrawService depositWithdrawService)
    {
        _bankingApiService = bankingApiService;
        _userManager = userManager;
        _depositWithdrawService = depositWithdrawService;
    }

    public IActionResult Deposit() => View();

    [HttpPost]
    public async Task<IActionResult> Deposit(decimal amount)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (amount <= 0)
        {
            return BadRequest("Deposit amount must be greater than zero.");
        }

        var userId = GetUserId();
        if (userId == null)
        {
            return Unauthorized("User is not authenticated.");
        }

        var depositWithdrawId = await _depositWithdrawService.AddDepositWithdrawAsync(
            userId, amount);
        if (depositWithdrawId is null)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error.");
        }

        var bankingApiResponse = await _bankingApiService.DepositBankingApiAsync(
            depositWithdrawId, amount);
        if (bankingApiResponse is null)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error.");
        }

        if (bankingApiResponse.Status == "Success" &&
            !string.IsNullOrWhiteSpace(bankingApiResponse.PaymentUrl))
        {
            return RedirectToAction(bankingApiResponse.PaymentUrl);
        }

        var errorMessage = bankingApiResponse.ErrorCode switch
        {
            "INVALID_DATA" => "The data provided for the deposit is invalid.",
            "INVALID_HASH" => "The request failed hash validation. Please try again.",
            _ => "An unknown error occurred. Please try again later."
        };

        return BadRequest(new { errorMessage });
    }

    public IActionResult Withdraw() => View();

    [HttpPost]
    public async Task<IActionResult> Withdraw(decimal amount)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (amount <= 0)
        {
            return BadRequest("Deposit amount must be greater than zero.");
        }

        var userId = GetUserId();
        if (userId == null)
        {
            return Unauthorized("User is not authenticated.");
        }

        var transaction = new DepositWithdrawRequest
        {
            UserId = userId,
            TransactionType = "Withdraw",
            Amount = amount,
            Status = "Pending",
            CreatedAt = DateTime.UtcNow,
        };
        /*var result = await _requestRepository.CreateAsync(transaction);

        if (result is null)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Internal sever error");
        } */

        return Ok();
    }

    public IActionResult TransactionHistory() => View();

    [HttpGet]
    public async Task<IActionResult> GetTransactionHistory()
    {
        var userId = GetUserId();
        if (userId == null)
        {
            return Unauthorized("User is not authenticated.");
        }

        //var transactions = await _requestRepository.GetAllAsync();

        return Json(new {  });
    }
}