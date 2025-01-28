using Microsoft.AspNetCore.Mvc;
using MvcProject.Data.Repositories;
using MvcProject.DTO;

namespace MvcProject.Controllers;

public class PaymentController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly IDepositWithdrawRequestRepository _requestRepository;
    private readonly ILogger<PaymentController> _logger;

    public PaymentController(IHttpClientFactory httpClientFactory, IConfiguration configuration,
        IDepositWithdrawRequestRepository requestRepository, ILogger<PaymentController> logger)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _requestRepository = requestRepository;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Pay(string transactionId)
    {
        _logger.LogInformation("Pay method called with transactionId: {TransactionId}", transactionId);

        if (string.IsNullOrEmpty(transactionId))
        {
            _logger.LogWarning("Transaction ID is required.");
            return BadRequest("Transaction ID is required.");
        }

        var model = new PaymentViewModel { TransactionId = transactionId };

        return View(model);
    }

    // Done
    [HttpPost]
    public async Task<IActionResult> Confirm(string transactionId)
    {
        _logger.LogInformation("Confirm method called with transactionId: {TransactionId}", transactionId);
        
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
                _logger.LogWarning("Payment rejected for transactionId: {TransactionId}", transactionId);
                return BadRequest(new { Message = "Payment has been rejected" });
            }

            _logger.LogInformation("Payment processed successfully for transactionId: {TransactionId}", transactionId);
            return Json(new { Message = "Payment has been processed successfully." });
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Internal error occurred while processing payment for transactionId: {TransactionId}", transactionId);
            return BadRequest(new { Message = "Internal Error.", Detail = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Cancel(string transactionId)
    {
        _logger.LogInformation("Cancel method called with transactionId: {TransactionId}", transactionId);

        try
        {
            await _requestRepository.DeleteAsync(transactionId);
            _logger.LogInformation("Payment cancelled successfully for transactionId: {TransactionId}", transactionId);
            return Json(new { Message = "Payment has been cancelled." });
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Failed to cancel payment for transactionId: {TransactionId}", transactionId);
            return BadRequest(new { Message = "Failed to update status.", Detail = ex.Message });
        }
    }
}