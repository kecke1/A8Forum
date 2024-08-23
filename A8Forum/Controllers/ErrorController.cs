using A8Forum.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace A8Forum.Controllers;

public class ErrorController : Controller
{
    private readonly ILogger<GauntletLeaderboardController> _logger;

    public ErrorController(ILogger<GauntletLeaderboardController> logger)
    {
        _logger = logger;
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}