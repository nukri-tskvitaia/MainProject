using BankingApi.Configuration;
using BankingApi.DTO;
using BankingApi.Helper;
using BankingApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Globalization;

namespace BankingApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DepositController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly ICallbackService _callbackService;
    private readonly List<MerchantSettings> _merchants;
    private readonly ILogger<DepositController> _logger;

    public DepositController(IConfiguration configuration, ICallbackService callbackService, IOptionsMonitor<List<MerchantSettings>> optionsMonitor, ILogger<DepositController> logger)
    {
        _configuration = configuration;
        _callbackService = callbackService;
        _merchants = optionsMonitor.CurrentValue;
        _logger = logger;
    }

    // Done
    [HttpPost("StartDeposit")]
    public async Task<IActionResult> Deposit([FromBody] DepositRequest request)
    {
        _logger.LogInformation("StartDeposit method called with request ID: {RequestId}", request.Id);

        var validationResult = RequestValidator.ValidateDeposit(_merchants, request.Amount, request.MerchantId);
        if (!validationResult)
        {
            _logger.LogWarning("Validation failed for deposit request ID: {RequestId}", request.Id);
            return BadRequest(new ErrorResponse
            {
                ErrorCode = "INVALID_DATA"
            });
        }
        
        var hashValidationResult = HashGenerator.GenerateHash(
            request.Amount,
            request.MerchantId,
            request.Id,
            _configuration["Secrets:Key"]!
            );
        if (hashValidationResult != request.Hash)
        {
            _logger.LogWarning("Hash validation failed for deposit request ID: {RequestId}", request.Id);
            return BadRequest(new ErrorResponse
            {
                ErrorCode = "INVALID_HASH"
            });
        }

        _logger.LogInformation("Deposit request validated successfully for request ID: {RequestId}", request.Id);
        return Ok(new Response
        {
            Status = "Success",
            PaymentUrl = $"{_configuration["Secrets:BasePaymentUrl"]!}{request.Id}"
        });
    }

    // Done
    [HttpPost("FinishDeposit")]
    public async Task<IActionResult> DepositFinish(string transactionId, [FromBody] AmountRequest request)
    {
        _logger.LogInformation("FinishDeposit method called with transaction ID: {TransactionId}", transactionId);

        bool isSuccess = int.Parse(request.Amount.ToString("0.###").Replace(",", ".").Replace(".", "")) % 2 == 0;
        var hash = HashGenerator.GenerateHash(
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
                Hash = hash,
                ClientId = _configuration["Secrets:ClientId"]!
            });
        if (response == null)
        {
            _logger.LogError("Callback service response is null for transaction ID: {TransactionId}", transactionId);
            return Ok(new { Message = "Error" });
        }
        
        if (!response.Status)
        {
            _logger.LogWarning("Deposit request rejected for transaction ID: {TransactionId} with error: {ErrorCode}", transactionId, response.ErrorCode);
            return BadRequest(new { Message = response.ErrorCode });
        }

        _logger.LogInformation("Deposit request processed successfully for transaction ID: {TransactionId}", transactionId);
        return Ok(new { Message = isSuccess ? "Success" : "Rejected" });
    }
}
