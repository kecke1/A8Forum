using System.Diagnostics;
using A8Forum.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace A8Forum.Controllers;

public class ErrorController(ILogger<GauntletLeaderboardController> logger) : Controller
{
    private readonly ILogger<GauntletLeaderboardController> _logger = logger;

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}