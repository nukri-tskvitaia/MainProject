using Microsoft.AspNetCore.Mvc;
using MvcProject.Data.Repositories;

namespace MvcProject.Controllers;

[Route("Payment")]
public class PaymentController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly IDepositWithdrawRequestRepository _requestRepository;

    public PaymentController(IHttpClientFactory httpClientFactory, IConfiguration configuration,
        IDepositWithdrawRequestRepository requestRepository)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _requestRepository = requestRepository;
    }

    [HttpGet]
    public IActionResult Payment(string transactionId)
    {
        if (string.IsNullOrEmpty(transactionId))
        {
            return BadRequest("Transaction ID is required.");
        }

        return View(model: transactionId);
    }

    [HttpPost]
    public async Task<IActionResult> Confirm(string transactionId)
    {
        var client = _httpClientFactory.CreateClient();
        var amount = await _requestRepository.GetAmountAsync(transactionId);
        var response = await client.PostAsJsonAsync($"{_configuration["BankingApiBaseUrl"]}/FinishDeposit?transactionId={transactionId}", new { Amount = amount });

        if (!response.IsSuccessStatusCode)
        {
            return Json(new { message = "Failed to complete payment." });
        }

        return Json(new { message = "Payment has been processed successfully." });
    }
}