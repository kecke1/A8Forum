using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Services;

namespace A8Forum.Controllers;

[Authorize]
public class DataManagementController(IDataManagementService dataManagementService) : Controller
{
    [HttpGet]
    public async Task<JsonResult> Export()
    {
        var export = await dataManagementService.ExportData();

        return Json(export);
    }
}