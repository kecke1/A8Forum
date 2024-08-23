using A8Forum.Areas.Identity.Data;
using A8Forum.Extensions;
using A8Forum.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Shared.Services;

namespace A8Forum.Controllers;

[Authorize]
public class GauntletRunsController : Controller
{
    private readonly IGauntletService _gauntletService;
    private readonly IMasterDataService _masterDataService;
    private readonly IAuthorizationService _authorizationService;
    private readonly UserManager<A8ForumazurewebsitesnetUser> _userManager;

    public GauntletRunsController(IMasterDataService masterDataService,
        IGauntletService gauntletService, IAuthorizationService authorizationService,
        UserManager<A8ForumazurewebsitesnetUser> userManager)
    {
        _gauntletService = gauntletService;
        _masterDataService = masterDataService;
        _authorizationService = authorizationService;
        _userManager = userManager;
    }

    private async Task PopulateVehiclesDropDownListAsync(string? vehicle1Id = null, string? vehicle2Id = null, string? vehicle3Id = null, string? vehicle4Id = null,
        string? vehicle5Id = null)
    {
        var q = (await _masterDataService.GetVehiclesAsync())
            .Select(x => x.ToVehicleViewModel())
            .OrderBy(x => x.MaxRank)
            .ToList();
        ViewBag.Vehicle1Id = q.ToSelectList(vehicle1Id);
        ViewBag.Vehicle2Id = q.ToSelectList(vehicle2Id);
        ViewBag.Vehicle3Id = q.ToSelectList(vehicle3Id);
        ViewBag.Vehicle4Id = q.ToSelectList(vehicle4Id);
        ViewBag.Vehicle5Id = q.ToSelectList(vehicle5Id);
    }

    private async Task PopulateTracksDropDownListAsync(string? trackId = null)
    {
        var q = (await _masterDataService
            .GetTracksAsync())
            .Select(y => y.ToTrackViewModel()).OrderBy(x => x.TrackName)
            .ToList();

        ViewBag.TrackId = q.ToSelectList(trackId);
    }

    private async Task PopulateMembersDropDownListAsync(string? memberId = null)
    {
        var q = await _masterDataService.GetMembersAsync();

        ViewBag.MemberId = q.Select(x => x.ToMemberViewModel()).OrderBy(y => y.MemberDisplayName)
            .OrderBy(x => x.MemberDisplayName)
            .ToList()
            .ToSelectList(memberId);
    }

    public async Task<IActionResult> Index(string? trackId = null, string? memberId = null, DateTime? InsertDateFrom = null, DateTime? InsertDateTo = null)
    {
        await PopulateTracksDropDownListAsync(trackId);
        await PopulateMembersDropDownListAsync(memberId);
        ViewData["InsertDateFrom"] = InsertDateFrom ?? DateTime.Now.AddYears(-1);
        ViewData["InsertDateTo"] = InsertDateTo ?? DateTime.Now;

        var query = (await _gauntletService.GetGauntletRunsAsync()).ToList();

        if (!string.IsNullOrEmpty(trackId) && trackId != "-1")
        {
            query = query.Where(x => x.Track.Id == trackId).ToList();
        }

        if (!string.IsNullOrEmpty(memberId) && memberId != "-1")
        {
            query = query.Where(x => x.Member.Id == memberId).ToList();
        }

        query = query.Where(x => x.Idate >= ((DateTime)ViewData["InsertDateFrom"]) && x.Idate <= ((DateTime)ViewData["InsertDateTo"]).AddDays(1)).ToList();

        var runs = query.Select(x => x.ToGauntletRunViewModel())
             .OrderByDescending(x => x.Idate)
             .ToList();

        return View(runs);
    }

    public async Task<IActionResult> Report()
    {
        return View(await _gauntletService.GetGauntletReportAsync());
    }

    public async Task<IActionResult> Details(string? id)
    {
        if (id == null)
            return NotFound();

        var gauntletDefence = await _gauntletService.GetGauntletRunAsync(id);
        if (gauntletDefence == null)
            return NotFound();

        return View(gauntletDefence.ToGauntletRunViewModel());
    }

    public async Task<IActionResult> Import()
    {
        await PopulateMembersDropDownListAsync();
        return View();
    }

    [Authorize(Policy = "AdminRole")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Import(
        Shared.Dto.GauntletImportDTO gauntletDefences)
    {
        if (ModelState.IsValid)
        {
            await _gauntletService.ImportGauntletRunsAsync(gauntletDefences);
            return RedirectToAction(nameof(Index));
        }

        await PopulateMembersDropDownListAsync(gauntletDefences.MemberId);

        return View(gauntletDefences);
    }

    public async Task<IActionResult> Create()
    {
        await PopulateVehiclesDropDownListAsync();
        await PopulateMembersDropDownListAsync();
        await PopulateTracksDropDownListAsync();

        var isAdmin = await _authorizationService.AuthorizeAsync(User, "GauntletAdminRole");
        ViewBag.IsAdmin = isAdmin.Succeeded;

        return View();
    }

    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = "GauntletUserRole")]
    public async Task<IActionResult> Create(
        [Bind("GauntletRunId,TimeString,Deleted, LapTimeVerified,Track,Vehicle1,Vehicle2,Vehicle3,Vehicle4,Vehicle5, Member, PostUrl, RunDate, MediaLink")]
        ViewModels.GauntletRunViewModel gauntletRun)
    {
        if (ModelState.IsValid)
        {
            var isAdmin = await _authorizationService.AuthorizeAsync(User, "GauntletAdminRole");
            if (!isAdmin.Succeeded)
            {
                var user = await _userManager.GetUserAsync(User);
                gauntletRun.Member = (await _masterDataService.GetMemberAsync(user.MemberId)).ToMemberViewModel();
            }
            await _gauntletService.AddGauntletRunAsync(gauntletRun.ToDto());
            return RedirectToAction(nameof(Index));
        }

        await PopulateMembersDropDownListAsync(gauntletRun.Member.MemberId);
        await PopulateTracksDropDownListAsync(gauntletRun.Track.TrackId);
        await PopulateVehiclesDropDownListAsync(gauntletRun.Vehicle1.VehicleId, gauntletRun.Vehicle2.VehicleId,
            gauntletRun.Vehicle3?.VehicleId, gauntletRun.Vehicle4?.VehicleId, gauntletRun.Vehicle5?.VehicleId);

        return View(gauntletRun);
    }

    public async Task<IActionResult> Edit(string? id)
    {
        if (id == null)
            return NotFound();

        var d = await _gauntletService.GetGauntletRunAsync(id);
        if (d == null)
            return NotFound();

        await PopulateVehiclesDropDownListAsync(d.Vehicle1.Id, d.Vehicle2.Id, d.Vehicle3.Id, d.Vehicle4?.Id, d.Vehicle5?.Id);
        await PopulateTracksDropDownListAsync(d.Track.Id);
        await PopulateMembersDropDownListAsync(d.Member.Id);

        return View(d.ToGauntletRunViewModel());
    }

    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = "GauntletAdminRole")]
    public async Task<IActionResult> Edit(string id,
        [Bind(
            "GauntletRunId,TimeString,Idate,Deleted,LapTimeVerified,Track,Vehicle1,Vehicle2,Vehicle3,Vehicle4,Vehicle5, Member, PostUrl, RunDate, MediaLink")]
        ViewModels.GauntletRunViewModel d)
    {
        if (id != d.GauntletRunId)
            return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                await _gauntletService.UpdateGauntletRunAsync(d.ToDto());
            }
            catch (Exception)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }

        await PopulateVehiclesDropDownListAsync(d.Vehicle1.VehicleId, d.Vehicle2.VehicleId, d.Vehicle3.VehicleId, d.Vehicle4?.VehicleId, d.Vehicle5?.VehicleId);
        await PopulateTracksDropDownListAsync(d.Track.TrackId);
        await PopulateMembersDropDownListAsync(d.Member.MemberId);
        return View(d);
    }

    public async Task<IActionResult> Delete(string? id)
    {
        if (id == null)
            return NotFound();

        var gauntletRun = await _gauntletService.GetGauntletRunAsync(id);
        if (gauntletRun == null)
            return NotFound();

        return View(gauntletRun.ToGauntletRunViewModel());
    }

    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = "GauntletAdminRole")]
    public async Task<IActionResult> DeleteConfirmed(string id)
    {
        await _gauntletService.DeleteGauntletRunAsync(id);
        return RedirectToAction(nameof(Index));
    }
}