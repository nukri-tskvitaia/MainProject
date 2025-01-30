using BankingApi.Configuration;
using BankingApi.DTO;
using BankingApi.Helper;
using BankingApi.Services;
using log4net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace BankingApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DepositController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly ICallbackService _callbackService;
    private readonly List<MerchantSettings> _merchants;
    private static readonly ILog _logger = LogManager.GetLogger(typeof(DepositController));

    public DepositController(
        IConfiguration configuration,
        ICallbackService callbackService,
        IOptionsMonitor<List<MerchantSettings>> optionsMonitor)
    {
        _configuration = configuration;
        _callbackService = callbackService;
        _merchants = optionsMonitor.CurrentValue;
    }

    // Done
    [HttpPost("StartDeposit")]
    public async Task<IActionResult> Deposit([FromBody] DepositRequest request)
    {
        _logger.InfoFormat("StartDeposit method called with request ID: {0}", request.Id);

        var validationResult = RequestValidator.ValidateMerchant(_merchants, request.MerchantId);
        if (!validationResult)
        {
            _logger.WarnFormat("Validation failed for deposit request ID: {0}", request.Id);
            return BadRequest(new ErrorResponse
            {
                ErrorCode = "INVALID_DATA"
            });
        }
        
        var generatedHash = HashGenerator.GenerateHash(
            request.Amount,
            request.MerchantId,
            request.Id,
            _configuration["Secrets:Key"]!
            );
        if (generatedHash != request.Hash)
        {
            _logger.WarnFormat("Hash validation failed for deposit request ID: {0}", request.Id);
            return BadRequest(new ErrorResponse
            {
                ErrorCode = "INVALID_HASH"
            });
        }

        _logger.InfoFormat("Deposit request validated successfully for request ID: {0}", request.Id);
        return Ok(new Response
        {
            Status = "Success",
            PaymentUrl = $"{_configuration["Secrets:BasePaymentUrl"]!}{request.Id}"
        });
    }

    // Done
    [HttpPost("FinishDeposit")]
    public async Task<IActionResult> DepositFinish([FromQuery] int transactionId, [FromBody] AmountRequest request)
    {
        _logger.InfoFormat("FinishDeposit method called with transaction ID: {0}", transactionId);

        bool isSuccess = transactionId % 2 == 0;
        var generatedHash = HashGenerator.GenerateHash(
            request.Amount,
            _configuration["Secrets:ClientId"]!,
            transactionId,
            _configuration["Secrets:Key"]!
            );

        var response = await _callbackService.NotifyMvcAsync(
            $"{_configuration["Secrets:MvcCallbackBaseUrl"]!}/HandleDeposit",
            new MvcCallbackResponse
            {
                TransactionId = transactionId,
                Amount = request.Amount,
                Status = isSuccess ? "Success" : "Rejected",
                Hash = generatedHash,
                ClientId = _configuration["Secrets:ClientId"]!
            });

        if (response == null)
        {
            _logger.ErrorFormat("Callback service response is null for transaction ID: {0}", transactionId);
            return Ok(new { Message = "Error" });
        }
        
        if (!response.Status)
        {
            _logger.WarnFormat("Deposit request rejected for transaction ID: {0} with error: {1}", transactionId, response.ErrorCode);
            return BadRequest(new { Message = response.ErrorCode });
        }

        _logger.InfoFormat("Deposit request processed successfully for transaction ID: {0}", transactionId);
        return Ok(new { Message = isSuccess ? "Success" : "Rejected" });
    }
}
