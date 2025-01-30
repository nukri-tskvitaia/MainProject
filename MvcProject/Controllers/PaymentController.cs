using log4net;
using Microsoft.AspNetCore.Mvc;
using MvcProject.Data.Repositories;
using MvcProject.DTO;

namespace MvcProject.Controllers;

public class PaymentController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly IDepositWithdrawRequestRepository _requestRepository;
    private static readonly ILog _logger = LogManager.GetLogger(typeof(PaymentController));

    public PaymentController(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        IDepositWithdrawRequestRepository requestRepository)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _requestRepository = requestRepository;
    }

    [HttpGet]
    public IActionResult Pay(int transactionId)
    {
        _logger.InfoFormat("Successfully processed withdraw callback for TransactionId: {0}", transactionId);

        if (!ModelState.IsValid)
        {
            _logger.Warn("Model state is invalid.");
            return BadRequest(new { Message = ModelState });
        }

        if (transactionId <= 0)
        {
            _logger.Warn("Transaction ID is required.");
            return BadRequest("Transaction ID is required.");
        }

        var model = new PaymentViewModel { TransactionId = transactionId };

        return View(model);
    }

    // Done
    [HttpPost]
    public async Task<IActionResult> Confirm(int transactionId)
    {
        _logger.InfoFormat("Successfully processed withdraw callback for TransactionId: {0}", transactionId);

        if (!ModelState.IsValid)
        {
            _logger.Warn("Model state is invalid.");
            return BadRequest(new { Message = ModelState });
        }

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
                _logger.InfoFormat("Payment rejected for transactionId: {0}", transactionId);
                return BadRequest(new { Message = "Payment has been rejected" });
            }

            _logger.InfoFormat("Payment processed successfully for transactionId: {0}", transactionId);
            return Json(new { Message = "Payment has been processed successfully." });
        }
        catch(Exception ex)
        {
            _logger.ErrorFormat("Internal error occurred while processing payment for transactionId: {0} - {1}", transactionId, ex.Message);
            return BadRequest(new { Message = "Internal Error.", Detail = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Cancel(int transactionId)
    {
        _logger.InfoFormat("Cancel method called with transactionId: {0}", transactionId);

        if (!ModelState.IsValid)
        {
            _logger.Warn("Model state is invalid.");
            return BadRequest(new { Message = ModelState });
        }

        try
        {
            await _requestRepository.UpdateStatusAsync(transactionId, "Rejected");
            _logger.InfoFormat("Payment cancelled successfully for transactionId: {0}", transactionId);
            return Json(new { Message = "Payment has been cancelled." });
        }
        catch(Exception ex)
        {
            _logger.ErrorFormat("Internal error occurred while processing payment for transactionId: {0} - {1}", transactionId, ex.Message);
            return BadRequest(new { Message = "Failed to update status.", Detail = ex.Message });
        }
    }
}