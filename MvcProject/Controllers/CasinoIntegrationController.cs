using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MvcProject.Data.Repositories;

namespace MvcProject.Controllers;

[Authorize(Roles = "Player")]
public class CasinoIntegrationController : BaseController
{
    private readonly ITokenRepository _tokenRepository;
    private static readonly ILog _logger = LogManager.GetLogger(typeof(CasinoIntegrationController));

    public CasinoIntegrationController(ITokenRepository tokenRepository)
    {
        _tokenRepository = tokenRepository;
    }

    public IActionResult PublicToken() => View();

    [HttpPost]
    public async Task<IActionResult> GeneratePublicToken()
    {
        _logger.Info("GeneratePublicToken method called");

        var userId = GetUserId();

        if (userId is null)
        {
            _logger.Warn("User not found.");
            return StatusCode(406, new { StatusCode = 406 });
        }

        try
        {
            _logger.Info("CreatePublicTokenAsync method called");
            var response = await _tokenRepository.CreatePublicTokenAsync(userId!);

            if (response == null || response.StatusCode != 200)
            {
                _logger.WarnFormat("Error occured - {0}", response!.ErrorMessage);
                return StatusCode(response!.StatusCode, new { response.StatusCode });
            }

            _logger.Info("CreatePublicTokenAsync method succeeded");
            return Json(response.PublicToken);
        }
        catch
        {
            _logger.Error("Internal Server error.");
            return StatusCode(500, new { StatusCode = 500 });
        }
    }
}
