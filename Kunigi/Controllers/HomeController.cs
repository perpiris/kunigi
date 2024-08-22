using Microsoft.AspNetCore.Mvc;

namespace Kunigi.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Error()
    {
        return View();
    }
}