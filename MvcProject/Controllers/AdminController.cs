using log4net;
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
    private static readonly ILog _logger = LogManager.GetLogger(typeof(AdminController));

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
        _logger.Info("Fetching pending requests.");
        var pendingRequests = await _requestRepository.GetPendingRequestsAsync("Pending", "Withdraw");

        return Json(pendingRequests);
    }

    [HttpPost]
    public async Task<IActionResult> ApproveRequest(int id)
    {
        _logger.InfoFormat("Attempting to approve request with ID: {0}", id);

        if (!ModelState.IsValid)
        {
            _logger.Warn("Model state is invalid.");
            return BadRequest(new { Message = ModelState });
        }

        var request = await _requestRepository.GetByIdAsync(id);

        if (request == null || request.Status != "Pending")
        {
            return Json(new { Message = "Invalid request." });
        }

        var bankRequest = await _bankingApiService.WithdrawBankingApiAsync(id, request.Amount);

        return Json(new { Message = bankRequest.Status ? "Request approved successfully." : bankRequest.Message! });
    }

    [HttpPost]
    public async Task<IActionResult> RejectRequest(int id)
    {
        _logger.InfoFormat("Attempting to reject request with ID: {0}", id);

        if (!ModelState.IsValid)
        {
            _logger.Warn("Model state is invalid.");
            return BadRequest(new { Message = ModelState });
        }

        var request = await _requestRepository.GetByIdAsync(id);

        if (request == null || request.Status != "Pending")
            return Json(new { Message = "Invalid request." });

        var response = await _requestRepository.UpdateStatusAsync(id, "Rejected");

        if (response.Status == "Failed")
        {
            _logger.ErrorFormat("Processing Withdraw Failed - {0}", response.ErrorMessage);
            return BadRequest(new { Message = "Failed update depositWithdraw" });
        }

        return Json(new { Message = "Request rejected successfully." });
    }
}
