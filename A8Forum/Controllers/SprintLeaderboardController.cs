using System.Diagnostics;
using A8Forum.Dto;
using A8Forum.Extensions;
using A8Forum.ViewModels;
using HtmlTableHelper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Shared.Services;

namespace A8Forum.Controllers;

public class SprintLeaderboardController(ILogger<SprintLeaderboardController> logger,
        ISprintService sprintService)
    : Controller
{
    private readonly ILogger<SprintLeaderboardController> _logger = logger;

    [HttpPost]
    [HttpGet]
    public async Task<IActionResult> Index(SprintLeaderboardViewModel? model)
    {
        var lb = await sprintService.GetSprintLeaderboardRowsAsync();
        var tableCols = lb.ToTableCols(model?.ShowAllRuns ?? false);
        var tableSetings = new HtmlTableSetting
        {
            IsHtmlEncodeMode = false
        };

        ViewBag.Table = tableCols.ToHtmlTable(HTMLTableSetting: tableSetings,
            tableAttributes: new
                { @class = "table table-bordered table-sm table-hover table-striped", id = "leaderboardTable" },
            thAttributes: new { @class = "table-dark bg-dark" });

        PopulateNameDropDownList(tableCols);
        PopulateTrackDropDownList(tableCols);
        if (!Response.Headers.ContainsKey("Cache-Control"))
            Response.Headers.Add("Cache-Control", "no-store");
        return View();
    }

    public async Task<IActionResult> Schedule()
    {

        var s = await sprintService.GetSprintScheduleAsync(DateTime.Now);

        return View(s);
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    private void PopulateNameDropDownList(IList<SprintLeaderboardTableColDto> cols)
    {
        var q = cols.Select(x => x.Name).Distinct().OrderBy(x => x).ToList();
        ViewBag.Names = new SelectList(q);
    }

    private void PopulateTrackDropDownList(IList<SprintLeaderboardTableColDto> cols)
    {
        var q = cols.Select(x => x.Track).Distinct().OrderBy(x => x).ToList();
        ViewBag.Tracks = new SelectList(q);
    }
}