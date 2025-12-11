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
using static Shared.Services.SprintService;

namespace A8Forum.Controllers;

public class SprintLeaderboardController(ILogger<SprintLeaderboardController> logger,
        ISprintService sprintService)
    : Controller
{
    private readonly ILogger<SprintLeaderboardController> _logger = logger;


    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] LeaderboardFilterInput filter)
    {
        var param = SprintLeaderboardParamsMapper.ToParams(filter);

        // TOTAL
        var totalOrdered = await sprintService.GetSprintTotalLeaderboardAsync(param);
        var total = totalOrdered?.ToList() ?? new List<SprintLeaderboardResultDto>();

        // BY TRACK
        var byTrackOrdered = await sprintService.GetSprintLeaderboardByTrack(param);
        var byTrackGroups = byTrackOrdered?.ToList() ?? new List<GroupedSprintLeaderboardRowsDto>();

        // BY MEMBER (Best Lap Times)
        var byMemberOrdered = await sprintService.GetSprintLeaderboardByMember(param);
        var bestLapGroups = byMemberOrdered?.ToList() ?? new List<GroupedSprintLeaderboardRowsDto>();

        var vm = new SprintLeaderboardViewModel
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
    public async Task<IActionResult> Runs(SprintRunsViewModel? model)
    {
        var lb = await sprintService.GetSprintLeaderboardRowsAsync(new GetSprintLeaderboardRowsParams());
        var tableCols = lb.ToTableCols(model?.ShowAllRuns ?? false);
        var tableSetings = new HtmlTableSetting
        {
            IsHtmlEncodeMode = false
        };

        if (model == null)
        {
            model = new SprintRunsViewModel();
        }

        //model.TodaysTrack = (await sprintService.GetSprintScheduleAsync(DateTime.Now)).Schedule.Where(x => x.);

        ViewBag.Table = tableCols.ToHtmlTable(HTMLTableSetting: tableSetings,
            tableAttributes: new
                { @class = "table table-bordered table-sm table-hover table-striped", id = "leaderboardTable" },
            thAttributes: new { @class = "table-dark bg-dark" });

        PopulateNameDropDownList(tableCols);
        PopulateTrackDropDownList(tableCols);
        if (!Response.Headers.ContainsKey("Cache-Control"))
            Response.Headers.Add("Cache-Control", "no-store");
        return View(model);
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