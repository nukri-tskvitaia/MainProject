using Microsoft.AspNetCore.Mvc;

namespace MvcProject.Controllers;

public class AdminController : Controller
{
    public IActionResult Dashboard() => View();
}
