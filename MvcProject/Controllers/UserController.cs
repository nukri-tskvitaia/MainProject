using Microsoft.AspNetCore.Mvc;

namespace MvcProject.Controllers;

public class UserController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public UserController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Profile()
    {
        return View();
    }
}
