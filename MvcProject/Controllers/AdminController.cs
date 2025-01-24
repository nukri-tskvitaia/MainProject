using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MvcProject.Data.Repositories;
using MvcProject.Services;

namespace MvcProject.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly IDepositWithdrawRequestRepository _requestRepository;
    private readonly IBankingApiService _bankingApiService;

    public AdminController(
        IDepositWithdrawRequestRepository requestRepository,
        IBankingApiService bankingApiService)
    {
        _requestRepository = requestRepository;
        _bankingApiService = bankingApiService;
    }

    public IActionResult Dashboard() => View();

    [HttpGet]
    public async Task<IActionResult> GetPendingRequests()
    {
        var pendingRequests = await _requestRepository.GetPendingRequestsAsync("Pending", "Withdraw");

        return Json(pendingRequests);
    }

    [HttpPost]
    public async Task<IActionResult> ApproveRequest(string id)
    {
        var request = await _requestRepository.GetByIdAsync(int.Parse(id));

        if (request == null || request.Status != "Pending")
        {
            return Json(new { Message = "Invalid request." });
        }

        var bankRequest = await _bankingApiService.WithdrawBankingApiAsync(id, request.Amount);

        return Json(new { Message = bankRequest.Status ? "Request approved successfully." : bankRequest.Message! });
    }

    [HttpPost]
    public async Task<IActionResult> RejectRequest(string id)
    {
        var request = await _requestRepository.GetByIdAsync(int.Parse(id));

        if (request == null || request.Status != "Pending")
            return Json(new { Message = "Invalid request." });

        var response = await _requestRepository.DeleteAsync(id);

        return Json(new { Message = response ? "Request rejected successfully." : "Failed." });
    }
}
