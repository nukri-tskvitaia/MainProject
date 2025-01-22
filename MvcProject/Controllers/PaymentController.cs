using Microsoft.AspNetCore.Mvc;

namespace MvcProject.Controllers;

[Route("Payment")]
public class PaymentController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public PaymentController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    [HttpGet]
    public IActionResult Index(string transactionId)
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
        var response = await client.PostAsync($"{_configuration["BankingApiBaseUrl"]}/FinishDeposit?transactionId={transactionId}", null);

        if (!response.IsSuccessStatusCode)
        {
            return Json(new { message = "Failed to complete payment." });
        }

        return Json(new { message = "Payment has been processed successfully." });
    }
}