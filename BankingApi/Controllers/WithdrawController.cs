using BankingApi.DTO;
using BankingApi.Helper;
using BankingApi.Services;
using log4net;
using Microsoft.AspNetCore.Mvc;

namespace BankingApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WithdrawController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly ICallbackService _callbackService;
    private static readonly ILog _logger = LogManager.GetLogger(typeof(WithdrawController));

    public WithdrawController(
        IConfiguration configuration,
        ICallbackService callbackService)
    {
        _configuration = configuration;
        _callbackService = callbackService;
    }

    [HttpPost("StartWithdraw")]
    public async Task<IActionResult> Withdraw([FromBody] WithdrawRequest request)
    {
        _logger.InfoFormat("Withdraw method called with request ID: {0}", request.Id);

        var validationResult = HashGenerator.GenerateHash(
            request.Amount,
            request.MerchantID,
            request.Id,
            _configuration["Secrets:Key"]!
        );

        if (validationResult != request.Hash)
        {
            _logger.WarnFormat("Hash validation failed for request ID: {0}", request.Id);
            return BadRequest(new 
            {
                Status = false
            });
        }

        _logger.InfoFormat("Hash validation succeeded for request ID: {0}", request.Id);
        bool isSuccess = request.Id % 2 == 0;

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
            _logger.ErrorFormat("Callback service response is null for request ID: {0}", request.Id);
            return Ok(new { Message = "Error" });
        }

        if (!response.Status)
        {
            _logger.WarnFormat("Withdraw request rejected for request ID: {0} with error: {1}", request.Id, response.ErrorCode);
            return BadRequest(new { Status = false, Message = response.ErrorCode });
        }

        _logger.InfoFormat("Withdraw request processed successfully for request ID: {0}", request.Id);
        return Ok(new
        {
            Status = true
        });
    }
}
