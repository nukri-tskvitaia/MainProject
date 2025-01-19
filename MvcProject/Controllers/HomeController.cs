using Microsoft.AspNetCore.Identity;
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

    public IActionResult Logout()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> GetUsername()
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

        var username = await _userRepository.GetUsernameAsync(userId);

        if (string.IsNullOrEmpty(username))
        {
            return NotFound(new { success = false, message = "Username not found." });
        }

        return Ok(new { success = true, name = username });
    }

    [HttpGet]
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
