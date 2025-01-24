using Microsoft.AspNetCore.Mvc;
using MvcProject.Data.Repositories;
using MvcProject.DTO;

namespace MvcProject.Controllers;

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
    public IActionResult Pay(string transactionId)
    {
        if (string.IsNullOrEmpty(transactionId))
        {
            return BadRequest("Transaction ID is required.");
        }

        var model = new PaymentViewModel { TransactionId = transactionId };

        return View(model);
    }

    // Done
    [HttpPost]
    public async Task<IActionResult> Confirm(string transactionId)
    {
        var client = _httpClientFactory.CreateClient();
        var amount = await _requestRepository.GetAmountAsync(transactionId);
        try
        {
            var response = await client.PostAsJsonAsync($"{_configuration["Secrets:BankingApiBaseUrl"]}/Deposit/FinishDeposit?transactionId={transactionId}",
                new { Amount = amount });
            var result = await response.Content.ReadFromJsonAsync<MessageModel>();
            var status= result?.Message;

            if (status != "Success")
            {
                return BadRequest(new { Message = "Payment has been rejected" });
                }

            return Json(new { Message = "Payment has been processed successfully." });
        }
        catch(Exception ex)
        {
            return BadRequest(new { Message = "Internal Error.", Detail = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Cancel(string transactionId)
    {
        try
        {
            await _requestRepository.DeleteAsync(transactionId);
            return Json(new { Message = "Payment has been cancelled." });
        }
        catch(Exception ex)
        {
            return BadRequest(new { Message = "Failed to update status.", Detail = ex.Message });
        }
    }
}