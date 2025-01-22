using BankingApi.DTO;
using BankingApi.Helper;
using Microsoft.AspNetCore.Mvc;

namespace BankingApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WithdrawController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public WithdrawController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpPost]
    public async Task<IActionResult> Withdraw([FromBody] WithdrawRequest request)
    {
        var validationResult = HashGenerator.GenerateWithdrawHash(
            request.Amount,
            request.MerchantID,
            request.TransactionID,
            request.UsersAccountNumber,
            request.UsersFullName,
            _configuration["Secrets:Key"]!
        );

        if (validationResult != request.Hash)
        {
            return BadRequest(new ErrorResponse
            {

            });
        }

        return Ok(new
        {
            Message = "Withdrawal processed successfully.",
        });
    }
}
