using BankingApi.Configuration;
using BankingApi.DTO;
using BankingApi.Helper;
using BankingApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace BankingApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DepositController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly ICallbackService _callbackService;
    private readonly List<string> _merchants;

    public DepositController(IConfiguration configuration, ICallbackService callbackService, IOptionsMonitor<MerchantSettings> optionsMonitor)
    {
        _configuration = configuration;
        _callbackService = callbackService;
        _merchants = optionsMonitor.CurrentValue.Merchants;
    }

    [HttpPost("Deposit")]
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
        
        var hashValidationResult = HashGenerator.GenerateDepositHash(
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

    [HttpPost("DepositFinish")]
    public async Task<IActionResult> DepositFinish([FromBody] DepositRequest request)
    {
        /*
        bool isSuccess = (int)(Math.Round(request.Amount, 2) * 100) % 2 == 0;

        var reponse = await _callbackService.NotifyMvcAsync(
            _configuration["Secrets:DepositCallback"]!,
            new MvcCallbackResponse
            {
                TransactionId = request.Id,
                Amount = request.Amount,
                Status = "Success"
            });
        */
        throw new NotImplementedException();
    }
}
