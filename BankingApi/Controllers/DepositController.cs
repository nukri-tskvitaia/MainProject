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

    public DepositController(IConfiguration configuration, ICallbackService callbackService, IOptionsMonitor<List<MerchantSettings>> optionsMonitor)
    {
        _configuration = configuration;
        _callbackService = callbackService;
        _merchants = optionsMonitor.CurrentValue;
    }

    // Done
    [HttpPost("StartDeposit")]
    public async Task<IActionResult> Deposit([FromBody] DepositRequest request)
    {
        var validationResult = RequestValidator.ValidateDeposit(_merchants, request.Amount, request.MerchantId);
        if (!validationResult)
        {
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
            return BadRequest(new ErrorResponse
            {
                ErrorCode = "INVALID_HASH"
            });
        }

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
            return Ok(new { Message = "Error" });
        }
        
        if (!response.Status)
        {
            return BadRequest(new { Message = response.ErrorCode });
        }

        return Ok(new { Message = isSuccess ? "Success" : "Rejected" });
    }
}
