using A8Forum.Dto;
using A8Forum.Extensions;
using A8Forum.ViewModels;
using HtmlTableHelper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Shared.Services;
using System.Data;
using System.Diagnostics;

namespace A8Forum.Controllers;

public class GauntletLeaderboardController : Controller
{
    private readonly ILogger<GauntletLeaderboardController> _logger;
    private readonly IGauntletService _gauntletService;

    public GauntletLeaderboardController(ILogger<GauntletLeaderboardController> logger, IGauntletService gauntletService)
    {
        _logger = logger;
        _gauntletService = gauntletService;
    }

    public async Task<IActionResult> Index()
    {
        var lb = await _gauntletService.GetGauntletLeaderboardRowsAsync();
        var tableCols = lb.ToTableCols();
        var tableSetings = new HtmlTableSetting
        {
            IsHtmlEncodeMode = false
        };

        ViewBag.Table = tableCols.ToHtmlTable(HTMLTableSetting: tableSetings,
             tableAttributes: new { @class = "table table-bordered table-sm table-hover table-striped", id = "leaderboardTable" }, thAttributes: new { @class = "table-dark bg-dark" });

        PopulateNameDropDownList(tableCols);
        PopulateTrackDropDownList(tableCols);
        if (!Response.Headers.ContainsKey("Cache-Control"))
        {
            Response.Headers.Add("Cache-Control", "no-store");
        }
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    private void PopulateNameDropDownList(IList<GauntletLeaderboardTableColDto> cols)
    {
        var q = cols.Select(x => x.Name).Distinct().OrderBy(x => x).ToList();
        ViewBag.Names = new SelectList(q);
    }

    private void PopulateTrackDropDownList(IList<GauntletLeaderboardTableColDto> cols)
    {
        var q = cols.Select(x => x.Track).Distinct().OrderBy(x => x).ToList();
        ViewBag.Tracks = new SelectList(q);
    }
}