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

    public WithdrawController(IConfiguration configuration, ICallbackService callbackService)
    {
        _configuration = configuration;
        _callbackService = callbackService;
    }

    [HttpPost("StartWithdraw")]
    public async Task<IActionResult> Withdraw([FromBody] WithdrawRequest request)
    {
        var validationResult = HashGenerator.GenerateHash(
            request.Amount,
            request.MerchantID,
            request.Id,
            _configuration["Secrets:Key"]!
        );

        if (validationResult != request.Hash)
        {
            return BadRequest(new 
            {
                Status = false
            });
        }

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
            return Ok(new { Message = "Error" });
        }

        if (!response.Status)
        {
            return BadRequest(new { Status = false, Message = response.ErrorCode });
        }

        return Ok(new
        {
            Status = true
        });
    }
}
