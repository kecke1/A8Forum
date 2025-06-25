using System.Diagnostics;
using A8Forum.ViewModels;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace A8Forum.Controllers;

public class ErrorController(ILogger<ErrorController> logger) : Controller
{
    private readonly ILogger<ErrorController> _logger = logger;

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        var exceptionHandlerPathFeature =
            HttpContext.Features.Get<IExceptionHandlerPathFeature>();
        return View(new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
            StackTrace = exceptionHandlerPathFeature.Error.StackTrace,
            ErrorMessage = exceptionHandlerPathFeature.Error.Message,
            ErrorInnerException = exceptionHandlerPathFeature.Error.InnerException?.Message
        });
    }
}