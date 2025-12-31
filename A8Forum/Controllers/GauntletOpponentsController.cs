using Microsoft.AspNetCore.Mvc;

namespace A8Forum.Controllers
{
    public class GauntletOpponentsController : Controller
    {
        [HttpGet]
        public IActionResult Index() => View();
    }
}