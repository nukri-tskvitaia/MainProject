using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MvcProject.Data.Repositories;
using MvcProject.Models;
using System.Diagnostics;
using System.Security.Claims;

namespace MvcProject.Controllers;

public class HomeController : Controller
{
    private readonly IUserRepository _userRepository;
    private readonly IWalletRepository _walletRepository;
    private static readonly ILog _logger = LogManager.GetLogger(typeof(HomeController));

    public HomeController(IUserRepository user, IWalletRepository walletRepository)
    {
        _userRepository = user;
        _walletRepository = walletRepository;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult Login()
    {
        return View();
    }

    public IActionResult Register()
    {
        return View();
    }

    [Authorize]
    public IActionResult Logout()
    {
        return View();
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetUsername()
    {
        if (!User?.Identity?.IsAuthenticated ?? true)
        {
            _logger.Warn("User is not authenticated.");
            return Unauthorized(new { success = false, message = "User is not authenticated." });
        }

        var userId = User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            _logger.Warn("User ID is missing or invalid.");
            return Unauthorized(new { success = false, message = "User ID is missing or invalid." });
        }

        _logger.InfoFormat("Fetching username for user ID: {0}", userId);
        var username = await _userRepository.GetUsernameAsync(userId);

        if (string.IsNullOrEmpty(username))
        {
            _logger.WarnFormat("Username not found for user ID: {0}", userId);
            return NotFound(new { success = false, message = "Username not found." });
        }

        _logger.InfoFormat("Username retrieved successfully for user ID: {0}", userId);
        return Ok(new { success = true, name = username });
    }

    [HttpGet]
    [Authorize(Roles = "Player")]
    public async Task<IActionResult> GetUserBalance()
    {
        if (!User?.Identity?.IsAuthenticated ?? true)
        {
            return Unauthorized(new { success = false, message = "User is not authenticated." });
        }

        var userId = User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return Unauthorized(new { success = false, message = "User ID is missing or invalid." });
        }

        var currencySymbol = await _walletRepository.GetCurrencyAsync(userId);
        var balance = await _walletRepository.GetUserBalanceAsync(userId);

        return Ok(new { success = true, symbol = currencySymbol, balance });
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
