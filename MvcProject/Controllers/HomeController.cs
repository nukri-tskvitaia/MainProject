using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MvcProject.Data.Repositories;
using MvcProject.Models;
using System.Diagnostics;
using System.Security.Claims;

namespace MvcProject.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IUserRepository _userRepository;
    private readonly IWalletRepository _walletRepository;

    public HomeController(ILogger<HomeController> logger, IUserRepository user, IWalletRepository walletRepository)
    {
        _logger = logger;
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
            _logger.LogWarning("User is not authenticated.");
            return Unauthorized(new { success = false, message = "User is not authenticated." });
        }

        var userId = User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            _logger.LogWarning("User ID is missing or invalid.");
            return Unauthorized(new { success = false, message = "User ID is missing or invalid." });
        }

        _logger.LogInformation("Fetching username for user ID: {UserId}", userId);
        var username = await _userRepository.GetUsernameAsync(userId);

        if (string.IsNullOrEmpty(username))
        {
            _logger.LogWarning("Username not found for user ID: {UserId}", userId);
            return NotFound(new { success = false, message = "Username not found." });
        }

        _logger.LogInformation("Username retrieved successfully for user ID: {UserId}", userId);
        return Ok(new { success = true, name = username });
    }

    [HttpGet]
    [Authorize(Roles = "Player")]
    public async Task<IActionResult> GetUserBalance()
    {
        if (!User?.Identity?.IsAuthenticated ?? true)
        {
            _logger.LogWarning("User is not authenticated.");
            return Unauthorized(new { success = false, message = "User is not authenticated." });
        }

        var userId = User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            _logger.LogWarning("User ID is missing or invalid.");
            return Unauthorized(new { success = false, message = "User ID is missing or invalid." });
        }

        _logger.LogInformation("Fetching balance and currency for user ID: {UserId}", userId);
        var currencySymbol = await _walletRepository.GetCurrencyAsync(userId);
        var balance = await _walletRepository.GetUserBalanceAsync(userId);

        _logger.LogInformation("Balance and currency retrieved successfully for user ID: {UserId}", userId);
        return Ok(new { success = true, symbol = currencySymbol, balance });
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
