using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MvcProject.Data.Repositories;
using MvcProject.Models;

namespace MvcProject.Controllers;

public class TransactionController : BaseController
{
    private readonly UserManager<User> _userManager;
    private readonly IDepositWithdrawRequestRepository _requestRepository;

    public TransactionController(UserManager<User> userManager, IDepositWithdrawRequestRepository requestRepository)
    {
        _userManager = userManager;
        _requestRepository = requestRepository;
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

        var transaction = new DepositWithdrawRequest
        {
            UserId = userId,
            TransactionType = "Deposit",
            Amount = amount,
            Status = "Pending",
            CreatedAt = DateTime.UtcNow,
        };
        var result = await _requestRepository.CreateAsync(transaction);

        if (result is null)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error.");
        }

        return Ok(result);

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
        var result = await _requestRepository.CreateAsync(transaction);

        if (result is null)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Internal sever error");
        }

        return Ok(result);
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

        var transactions = await _requestRepository.GetAllAsync();

        return Json(new { data = transactions });
    }
}