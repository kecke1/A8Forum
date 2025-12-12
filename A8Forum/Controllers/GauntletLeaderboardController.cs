using A8Forum.Dto;
using A8Forum.Extensions;
using A8Forum.Mappers;
using A8Forum.ViewModels;
using HtmlTableHelper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Shared.Dto;
using Shared.Params;
using Shared.Services;
using System.Diagnostics;

namespace A8Forum.Controllers;

public class GauntletLeaderboardController(ILogger<GauntletLeaderboardController> logger,
        IGauntletService gauntletService)
    : Controller
{
    private readonly ILogger<GauntletLeaderboardController> _logger = logger;


    [HttpPost]
    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] GauntletLeaderboardFilterInput filter)
    {
        var param = GauntletLeaderboardParamsMapper.ToParams(filter);

        // TOTAL
        var totalOrdered = await gauntletService.GetGauntletTotalLeaderboardAsync(param);
        var total = totalOrdered?.ToList() ?? new List<GauntletService.GauntletLeaderboardResultDto>();

        // BY TRACK
        var byTrackOrdered = await gauntletService.GetGauntletLeaderboardByTrack(param);
        var byTrackGroups = byTrackOrdered?.ToList() ?? new List<GroupedGauntletLeaderboardRowsDto>();

        // BY MEMBER (Best Lap Times)
        var byMemberOrdered = await gauntletService.GetGauntletLeaderboardByMember(param);
        var bestLapGroups = byMemberOrdered?.ToList() ?? new List<GroupedGauntletLeaderboardRowsDto>();

        var vm = new GauntletLeaderboardViewModel
        {
            Filter = filter,

            TotalResults = total,
            TotalCount = total.Count,

            ByTrackGroups = byTrackGroups,
            ByTrackCount = byTrackGroups.Sum(g => g.Rows.Count()),

            BestLapGroups = bestLapGroups,
            BestLapCount = bestLapGroups.Sum(g => g.Rows.Count())
        };

        return View(vm);
    }

    [HttpPost]
    [HttpGet]
    public async Task<IActionResult> Runs(GauntletRunsViewModel? model)
    {
        var lb = await gauntletService.GetGauntletLeaderboardRowsAsync(new GetGauntletLeaderboardRowsParams());
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