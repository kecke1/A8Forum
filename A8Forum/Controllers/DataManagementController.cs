using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Services;

namespace A8Forum.Controllers;

[Authorize(Policy = "AdminRole")]
public class DataManagementController : Controller
{
    private readonly IDataManagementService _dataManagementService;

    public DataManagementController(IDataManagementService dataManagementService)
    {
        _dataManagementService = dataManagementService;
    }

    [HttpGet]
    public async Task<JsonResult> Export()
    {
        var export = await _dataManagementService.ExportData();

        return Json(export);
    }
}