using BankingApi.DTO;
using BankingApi.Helper;
using BankingApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BankingApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WithdrawController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly ICallbackService _callbackService;
    private readonly ILogger<WithdrawController> _logger;

    public WithdrawController(IConfiguration configuration, ICallbackService callbackService, ILogger<WithdrawController> logger)
    {
        _configuration = configuration;
        _callbackService = callbackService;
        _logger = logger;
    }

    [HttpPost("StartWithdraw")]
    public async Task<IActionResult> Withdraw([FromBody] WithdrawRequest request)
    {
        _logger.LogInformation("Withdraw method called with request ID: {RequestId}", request.Id);

        var validationResult = HashGenerator.GenerateHash(
            request.Amount,
            request.MerchantID,
            request.Id,
            _configuration["Secrets:Key"]!
        );

        if (validationResult != request.Hash)
        {
            _logger.LogWarning("Hash validation failed for request ID: {RequestId}", request.Id);
            return BadRequest(new 
            {
                Status = false
            });
        }

        _logger.LogInformation("Hash validation succeeded for request ID: {RequestId}", request.Id);
        bool isSuccess = int.Parse(request.Amount.ToString("0.###").Replace(",", ".").Replace(".", "")) % 2 == 0;

        var newHash = HashGenerator.GenerateHash(
            request.Amount,
            _configuration["Secrets:ClientId"]!,
            request.Id,
            _configuration["Secrets:Key"]!
        );
        var response = await _callbackService.NotifyMvcAsync(
            $"{_configuration["Secrets:MvcCallbackBaseUrl"]!}/HandleWithdraw",
            new MvcCallbackResponse
            {
                TransactionId = request.Id,
                Amount = request.Amount,
                Status = isSuccess ? "Success" : "Rejected",
                Hash = newHash,
                ClientId = _configuration["Secrets:ClientId"]!
            });

        if (response == null)
        {
            _logger.LogError("Callback service response is null for request ID: {RequestId}", request.Id);
            return Ok(new { Message = "Error" });
        }

        if (!response.Status)
        {
            _logger.LogWarning("Withdraw request rejected for request ID: {RequestId} with error: {ErrorCode}", request.Id, response.ErrorCode);
            return BadRequest(new { Status = false, Message = response.ErrorCode });
        }

        _logger.LogInformation("Withdraw request processed successfully for request ID: {RequestId}", request.Id);
        return Ok(new
        {
            Status = true
        });
    }
}
