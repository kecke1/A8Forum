using A8Forum.Areas.Identity.Data;
using A8Forum.Extensions;
using A8Forum.Mappers;
using A8Forum.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Shared.Dto;
using Shared.Services;

namespace A8Forum.Controllers;

public class SprintRunsController(IMasterDataService masterDataService,
        ISprintService sprintService, IAuthorizationService authorizationService,
        UserManager<A8ForumazurewebsitesnetUser> userManager)
    : Controller
{
    private async Task PopulateVehiclesDropDownListAsync(string? vehicleId = null)
    {
        var q = (await masterDataService.GetVehiclesAsync())
            .Select(x => x.ToVehicleViewModel())
            .OrderBy(x => x.MaxRank)
            .ToList();
        ViewBag.VehicleId = q.ToSelectList(vehicleId);
    }

    private async Task PopulateTracksDropDownListAsync(string? trackId = null)
    {
        var q = (await masterDataService
                .GetTracksAsync(false, true))
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

    [Authorize]
    public async Task<IActionResult> Index(string? trackId = null, string? memberId = null,
        DateTime? InsertDateFrom = null, DateTime? InsertDateTo = null)
    {
        await PopulateTracksDropDownListAsync(trackId);
        await PopulateMembersDropDownListAsync(memberId);
        ViewData["InsertDateFrom"] = InsertDateFrom ?? DateTime.Now.AddYears(-1);
        ViewData["InsertDateTo"] = InsertDateTo ?? DateTime.Now;

        var query = (await sprintService.GetSprintRunsAsync()).ToList();

        if (!string.IsNullOrEmpty(trackId) && trackId != "-1")
            query = query.Where(x => x.Track.Id == trackId).ToList();

        if (!string.IsNullOrEmpty(memberId) && memberId != "-1")
            query = query.Where(x => x.Member.Id == memberId).ToList();

        query = query.Where(x =>
            x.Idate >= (DateTime)ViewData["InsertDateFrom"] &&
            x.Idate <= ((DateTime)ViewData["InsertDateTo"]).AddDays(1)).ToList();

        var runs = query.Select(x => x.ToSprintRunViewModel())
            .OrderByDescending(x => x.Idate)
            .ToList();

        return View(runs);
    }

    [Authorize]
    public async Task<IActionResult> Report()
    {
        return View(await sprintService.GetSprintReportAsync());
    }
    [Authorize]

    public async Task<IActionResult> Details(string? id)
    {
        if (id == null)
            return NotFound();

        var gauntletDefence = await sprintService.GetSprintRunAsync(id);
        if (gauntletDefence == null)
            return NotFound();

        return View(gauntletDefence.ToSprintRunViewModel());
    }


    public async Task<IActionResult> Schedule()
    {

        var s = await sprintService.GetSprintScheduleAsync(DateTime.Now);

        return View(s);
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
        SprintImportDTO sprintRuns)
    {
        if (ModelState.IsValid)
        {
            await sprintService.ImportSprintRunsAsync(sprintRuns);
            return RedirectToAction(nameof(Index));
        }

        await PopulateMembersDropDownListAsync(sprintRuns.MemberId);

        return View(sprintRuns);
    }

    [Authorize]
    public async Task<IActionResult> Create()
    {
        await PopulateVehiclesDropDownListAsync();
        await PopulateMembersDropDownListAsync();
        await PopulateTracksDropDownListAsync();

        var isAdmin = await authorizationService.AuthorizeAsync(User, "SprintAdminRole");
        ViewBag.IsAdmin = isAdmin.Succeeded;

        return View();
    }

    [Authorize]
    public IActionResult CreateFromTemplate()
    {
        return View();
    }

    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = "SprintUserRole")]
    public async Task<IActionResult> CreateFromTemplate(
        [Bind(
            "PostUrl,TemplateText")]
        CreateSprintRunFromTemplateViewModel template)
    {
        await PopulateVehiclesDropDownListAsync();
        await PopulateMembersDropDownListAsync();
        await PopulateTracksDropDownListAsync();

        var isAdmin = await authorizationService.AuthorizeAsync(User, "SprintAdminRole");
        ViewBag.IsAdmin = isAdmin.Succeeded;
        return View("Create", (await sprintService
                .GetSprintRunFromTemplateAsync(template.TemplateText, template.PostUrl))
            .ToEditSprintRunViewModel());
    }

    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = "SprintUserRole")]
    public async Task<IActionResult> Create(
        [Bind(
            "SprintRunId,TimeString,TrackId,VehicleId, MemberId, PostUrl, RunDate, MediaLink, Save, VipLevel")]
        EditSprintRunViewModel sprintRun)
    {
        var isAdmin = await authorizationService.AuthorizeAsync(User, "SprintAdminRole");
        if (ModelState.IsValid && sprintRun.Save)
        {
            if (!isAdmin.Succeeded || string.IsNullOrEmpty(sprintRun.MemberId))
            {
                var user = await userManager.GetUserAsync(User);
                sprintRun.MemberId = (await masterDataService.GetMemberAsync(user.MemberId)).Id;
            }

            await sprintService.AddSprintRunAsync(sprintRun.ToDto());
            return RedirectToAction(nameof(Index));
        }

        await PopulateMembersDropDownListAsync(sprintRun.MemberId);
        await PopulateTracksDropDownListAsync(sprintRun.TrackId);
        await PopulateVehiclesDropDownListAsync(sprintRun.VehicleId);

        ViewBag.IsAdmin = isAdmin.Succeeded;

        return View(sprintRun);
    }

    [Authorize]
    public async Task<IActionResult> EditReferencePoint()
    {
        var r = await sprintService.GetSprintTrackReferencePointAsync();
        await PopulateTracksDropDownListAsync(r?.Track.Id);
        return View(r?.ToSprintTrackReferencePointViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = "AdminRole")]
    public async Task<IActionResult> EditReferencePoint(SprintTrackReferencePointViewModel r)
    {
        if (ModelState.IsValid && r.Track.TrackId != null)
        {
            try
            {
                await sprintService.UpsertSprintTrackReferencePointAsync(r.ToDto());
            }
            catch (Exception)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }

        await PopulateTracksDropDownListAsync(r?.Track.TrackId);
        return View(r);
    }

    [Authorize]
    public async Task<IActionResult> Edit(string? id)
    {
        if (id == null)
            return NotFound();

        var d = await sprintService.GetSprintRunAsync(id);
        if (d == null)
            return NotFound();

        await PopulateVehiclesDropDownListAsync(d.Vehicle.Id);
        await PopulateTracksDropDownListAsync(d.Track.Id);
        await PopulateMembersDropDownListAsync(d.Member.Id);

        return View(d.ToEditSprintRunViewModel());
    }

    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = "SprintAdminRole")]
    public async Task<IActionResult> Edit(string id,
        [Bind(
            "SprintRunId,TimeString,Idate,Deleted,TrackId,VehicleId, MemberId, PostUrl, RunDate, MediaLink, VipLevel")]
        EditSprintRunViewModel d)
    {
        if (id != d.SprintRunId)
            return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                await sprintService.UpdateSprintRunAsync(d.ToDto());
            }
            catch (Exception)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }

        await PopulateVehiclesDropDownListAsync(d.VehicleId);
        await PopulateTracksDropDownListAsync(d.TrackId);
        await PopulateMembersDropDownListAsync(d.MemberId);
        return View(d);
    }

    [Authorize]
    public async Task<IActionResult> Delete(string? id)
    {
        if (id == null)
            return NotFound();

        var gauntletRun = await sprintService.GetSprintRunAsync(id);
        if (gauntletRun == null)
            return NotFound();

        return View(gauntletRun.ToSprintRunViewModel());
    }

    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = "SprintAdminRole")]
    public async Task<IActionResult> DeleteConfirmed(string id)
    {
        await sprintService.DeleteSprintRunAsync(id);
        return RedirectToAction(nameof(Index));
    }
}