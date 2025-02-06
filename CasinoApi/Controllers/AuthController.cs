using CasinoApi.DTO;
using CasinoApi.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace CasinoApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ITokenRepository _tokenRepository;

    public AuthController(ITokenRepository tokenRepository)
    {
        _tokenRepository = tokenRepository;
    }

    [HttpPost]
    public async Task<IActionResult> Auth([FromBody] TokenRequest request)
    {
        if (request is null || !ModelState.IsValid ||
            string.IsNullOrWhiteSpace(request.PublicToken))
        {
            return StatusCode(411, new { StatusCode = 411 });
        }

        try
        {
            var response = await _tokenRepository.AuthorizeAsync(request);

            if (response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, new { response.StatusCode });
            }

            return Ok(new { response.StatusCode, response.Data });
        }
        catch
        {
            return StatusCode(500, new { StatusCode = 500 });
        }
    }
}
