using A8Forum.Areas.Identity.Data;
using A8Forum.Extensions;
using A8Forum.Mappers;
using A8Forum.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Shared.Dto;
using Shared.Models;
using Shared.Services;

namespace A8Forum.Controllers;

[Authorize]
public class GauntletRunsController(IMasterDataService masterDataService,
        IGauntletService gauntletService, IAuthorizationService authorizationService,
        UserManager<A8ForumazurewebsitesnetUser> userManager)
    : Controller
{
    private async Task PopulateVehiclesDropDownListAsync(string? vehicle1Id = null, string? vehicle2Id = null,
        string? vehicle3Id = null, string? vehicle4Id = null,
        string? vehicle5Id = null)
    {
        var q = (await masterDataService.GetVehiclesAsync())
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
        var q = (await masterDataService
                .GetTracksAsync())
            .Select(y => y.ToTrackViewModel()).OrderBy(x => x.TrackName)
            .ToList();

        ViewBag.TrackId = q.ToSelectList(trackId);
    }

    private async Task PopulateMembersDropDownListAsync(string? memberId = null)
    {
        var q = await masterDataService.GetMembersAsync();

        ViewBag.MemberId = q.Select(x => x.ToMemberViewModel()).OrderBy(y => y.MemberDisplayName)
            .OrderBy(x => x.MemberDisplayName)
            .ToList()
            .ToSelectList(memberId);
    }

    public async Task<IActionResult> Index(string? trackId = null, string? memberId = null,
        DateTime? InsertDateFrom = null, DateTime? InsertDateTo = null)
    {
        await PopulateTracksDropDownListAsync(trackId);
        await PopulateMembersDropDownListAsync(memberId);
        ViewData["InsertDateFrom"] = InsertDateFrom ?? DateTime.Now.AddYears(-1);
        ViewData["InsertDateTo"] = InsertDateTo ?? DateTime.Now;

        var query = (await gauntletService.GetGauntletRunsAsync()).ToList();

        if (!string.IsNullOrEmpty(trackId) && trackId != "-1")
            query = query.Where(x => x.Track.Id == trackId).ToList();

        if (!string.IsNullOrEmpty(memberId) && memberId != "-1")
            query = query.Where(x => x.Member.Id == memberId).ToList();

        query = query.Where(x =>
            x.Idate >= (DateTime)ViewData["InsertDateFrom"] &&
            x.Idate <= ((DateTime)ViewData["InsertDateTo"]).AddDays(1)).ToList();

        var runs = query.Select(x => x.ToGauntletRunViewModel())
            .OrderByDescending(x => x.Idate)
            .ToList();

        return View(runs);
    }

    public async Task<IActionResult> Report()
    {
        return View(await gauntletService.GetGauntletReportAsync());
    }

    public async Task<IActionResult> Details(string? id)
    {
        if (id == null)
            return NotFound();

        var gauntletDefence = await gauntletService.GetGauntletRunAsync(id);
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
        GauntletImportDTO gauntletDefences)
    {
        if (ModelState.IsValid)
        {
            await gauntletService.ImportGauntletRunsAsync(gauntletDefences);
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

        var isAdmin = await authorizationService.AuthorizeAsync(User, "GauntletAdminRole");
        ViewBag.IsAdmin = isAdmin.Succeeded;

        return View();
    }

    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = "GauntletUserRole")]
    public async Task<IActionResult> Create(
        [Bind(
            "GauntletRunId,TimeString, LapTimeVerified,TrackId,Vehicle1Id,Vehicle2Id,Vehicle3Id,Vehicle4Id,Vehicle5Id, MemberId, PostUrl, RunDate, MediaLink")]
        EditGauntletRunViewModel gauntletRun)
    {
        var isAdmin = await authorizationService.AuthorizeAsync(User, "GauntletAdminRole");
        if (ModelState.IsValid)
        {
            if (!isAdmin.Succeeded || string.IsNullOrEmpty(gauntletRun.MemberId))
            {
                var user = await userManager.GetUserAsync(User);
                gauntletRun.MemberId = (await masterDataService.GetMemberAsync(user.MemberId)).Id;
            }

            await gauntletService.AddGauntletRunAsync(gauntletRun.ToDto());
            return RedirectToAction(nameof(Index));
        }

        await PopulateMembersDropDownListAsync(gauntletRun.MemberId);
        await PopulateTracksDropDownListAsync(gauntletRun.TrackId);
        await PopulateVehiclesDropDownListAsync(gauntletRun.Vehicle1Id, gauntletRun.Vehicle2Id,
            gauntletRun.Vehicle3Id, gauntletRun.Vehicle4Id, gauntletRun.Vehicle5Id);

        ViewBag.IsAdmin = isAdmin.Succeeded;

        return View(gauntletRun);
    }

    public async Task<IActionResult> Edit(string? id)
    {
        if (id == null)
            return NotFound();

        var d = await gauntletService.GetGauntletRunAsync(id);
        if (d == null)
            return NotFound();

        await PopulateVehiclesDropDownListAsync(d.Vehicle1.Id, d.Vehicle2.Id, d.Vehicle3.Id, d.Vehicle4?.Id,
            d.Vehicle5?.Id);
        await PopulateTracksDropDownListAsync(d.Track.Id);
        await PopulateMembersDropDownListAsync(d.Member.Id);

        return View(d.ToEditGauntletRunViewModel());
    }

    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = "GauntletAdminRole")]
    public async Task<IActionResult> Edit(string id,
        [Bind(
            "GauntletRunId,TimeString,Idate,Deleted,LapTimeVerified,TrackId,Vehicle1Id,Vehicle2Id,Vehicle3Id,Vehicle4Id,Vehicle5Id, MemberId, PostUrl, RunDate, MediaLink")]
        EditGauntletRunViewModel d)
    {
        if (id != d.GauntletRunId)
            return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                await gauntletService.UpdateGauntletRunAsync(d.ToDto());
            }
            catch (Exception)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }

        await PopulateVehiclesDropDownListAsync(d.Vehicle1Id, d.Vehicle2Id, d.Vehicle3Id,
            d.Vehicle4Id, d.Vehicle5Id);
        await PopulateTracksDropDownListAsync(d.TrackId);
        await PopulateMembersDropDownListAsync(d.MemberId);
        return View(d);
    }

    public async Task<IActionResult> Delete(string? id)
    {
        if (id == null)
            return NotFound();

        var gauntletRun = await gauntletService.GetGauntletRunAsync(id);
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
        await gauntletService.DeleteGauntletRunAsync(id);
        return RedirectToAction(nameof(Index));
    }
}