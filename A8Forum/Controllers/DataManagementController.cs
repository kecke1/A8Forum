using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Services;
using System.Text.Json;
using Shared.Dto;

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

    [HttpGet]
    public async Task Import(string fileName)
    {
        var content = System.IO.File.ReadAllText(fileName);
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        var data = JsonSerializer.Deserialize<ExportDataDto>(content, options);
        await dataManagementService.ImportData(data);

        Redirect("/");
    }
}