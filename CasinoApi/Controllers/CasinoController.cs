using CasinoApi.DTO;
using CasinoApi.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace CasinoApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CasinoController : ControllerBase
{
    private readonly ICasinoRepository _casinoRepository;

    public CasinoController(ICasinoRepository casinoRepository)
    {
        _casinoRepository = casinoRepository;
    }

    [HttpPost("Bet")]
    public async Task<IActionResult> Bet([FromBody] BetRequest bet)
    {
        if (bet is null || !ModelState.IsValid)
        {
            return StatusCode(411, new { StatusCode = 411 });
        }

        if (string.IsNullOrWhiteSpace(bet.Token))
        {
            return StatusCode(404, new { StatusCode = 404 });
        }

        if (bet.Amount <= 0)
        {
            return StatusCode(407, new { StatusCode = 407 });
        }

        try
        {
            var response = await _casinoRepository.MakeBetAsync(bet);

            if (response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, new { response.StatusCode });
            }

            return Ok(response);

        }
        catch
        {
            return StatusCode(500, new { StatusCode = 500 });
        }
    }

    [HttpPost("Win")]
    public async Task<IActionResult> Win([FromBody] WinRequest win)
    {
        if (win is null || !ModelState.IsValid || win.Amount <= 0 || string.IsNullOrWhiteSpace(win.Token))
        {
            return StatusCode(411, new { StatusCode = 411 });
        }

        try
        {
            var response = await _casinoRepository.WinAsync(win);

            if (response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, new { response.StatusCode });
            }

            return Ok(response);

        }
        catch
        {
            return StatusCode(500, new { StatusCode = 500 });
        }
    }

    [HttpPost("CancelBet")]
    public async Task<IActionResult> CancelBet([FromBody] CancelBetRequest cancelBet)
    {
        if (cancelBet is null || !ModelState.IsValid || string.IsNullOrWhiteSpace(cancelBet.Token))
        {
            return StatusCode(411, new { StatusCode = 411 });
        }

        try
        {
            var response = await _casinoRepository.CancelBetAsync(cancelBet);

            if (response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, new { response.StatusCode });
            }

            return Ok(response);

        }
        catch
        {
            return StatusCode(500, new { StatusCode = 500 });
        }
    }

    [HttpPost("ChangeWin")]
    public async Task<IActionResult> ChangeWin([FromBody] ChangeWinRequest changeWin)
    {
        if (changeWin is null || !ModelState.IsValid || string.IsNullOrWhiteSpace(changeWin.Token))
        {
            return StatusCode(411, new { StatusCode = 411 });
        }

        try
        {
            var response = await _casinoRepository.ChangeWinAsync(changeWin);

            if (response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, new { response.StatusCode });
            }

            return Ok(response);

        }
        catch
        {
            return StatusCode(500, new { StatusCode = 500 });
        }
    }

    [HttpPost("Balance")]
    public async Task<IActionResult> GetBalance([FromBody] GetBalanceRequest getBalance)
    {
        if (getBalance is null || !ModelState.IsValid || string.IsNullOrWhiteSpace(getBalance.Token))
        {
            return StatusCode(411, new { StatusCode = 411 });
        }

        try
        {
            var response = await _casinoRepository.GetBalanceAsync(getBalance);

            if (response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, new { response.StatusCode });
            }

            return Ok(response);

        }
        catch
        {
            return StatusCode(500, new { StatusCode = 500 });
        }
    }

    [HttpPost("PlayerInfo")]
    public async Task<IActionResult> GetPlayerInfo([FromBody] GetUserInfoRequest getUserInfo)
    {
        if (getUserInfo is null || !ModelState.IsValid || string.IsNullOrWhiteSpace(getUserInfo.Token))
        {
            return StatusCode(411, new { StatusCode = 411 });
        }

        try
        {
            var response = await _casinoRepository.GetPlayerInfoAsync(getUserInfo);

            if (response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, new { response.StatusCode });
            }

            return Ok(response);

        }
        catch
        {
            return StatusCode(500, new { StatusCode = 500 });
        }
    }
}
