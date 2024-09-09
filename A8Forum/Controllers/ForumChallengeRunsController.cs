using A8Forum.Extensions;
using A8Forum.Mappers;
using A8Forum.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Shared.Dto;
using Shared.Extensions;
using Shared.Services;

namespace A8Forum.Controllers;

[Authorize]
public class ForumChallengeRunsController(IMasterDataService masterDataService,
        IForumChallengeService forumChallengeService)
    : Controller
{
    public async Task<IActionResult> Index(string? forumChallengeId = null)
    {
        var cId = forumChallengeId ?? (await forumChallengeService
                .GetForumChallengesAsync())
            .Where(x => !x.Deleted).MaxBy(x => x.EndDate)?.Id;

        ViewData["FilteredChallengeId"] = cId;

        await PopulateChallengesDropDownListAsync(cId);

        var runs = !string.IsNullOrEmpty(cId)
            ? (await forumChallengeService.GetForumChallengeRunsAsync(cId)).OrderByDescending(x => x.Idate).ToArray()
            : Array.Empty<ForumChallengeRunDTO>();

        try
        {
            if (!string.IsNullOrEmpty(cId))
            {
                await forumChallengeService.GetForumChallengeAsync(cId);
                var r = await forumChallengeService.GetForumChallengeRunsAsync(cId);
                var lb = forumChallengeService.GetForumChallengeLeaderBoardDto(r);
                var leaderBoard = forumChallengeService.GetForumChallengeLeaderboard(lb);
                ViewData["LeaderboardHtml"] = leaderBoard.ToHtml();

                ViewData["LeaderboardBB"] = leaderBoard;
            }
        }
        catch (Exception)
        {
            ViewData["LeaderboardHtml"] = "";
            ViewData["LeaderboardBB"] = "";
        }

        return View(runs.Select(x => x.ToForumChallengeRunViewModel()));
    }

    public async Task<IActionResult> Details(string? id)
    {
        if (id == null)
            return NotFound();

        var run = await forumChallengeService.GetForumChallengeRunAsync(id);
        if (run == null)
            return NotFound();

        return View(run.ToForumChallengeRunViewModel());
    }

    public async Task<IActionResult> Create()
    {
        await PopulateChallengesDropDownListAsync();
        await PopulateMembersDropDownListAsync();
        await PopulateVehiclesDropDownListAsync();
        return View();
    }

    private async Task PopulateChallengesDropDownListAsync(string? challengeId = null)
    {
        var c = (await forumChallengeService
                .GetForumChallengesAsync())
            .OrderByDescending(x => x.EndDate);

        ViewBag.ChallengeId = new SelectList(c, "Id", "Title", challengeId ?? c.FirstOrDefault()?.Id);
    }

    private async Task PopulateMembersDropDownListAsync(string? memberId = null)
    {
        var q = (await masterDataService.GetMembersAsync()).Select(x => x.ToMemberViewModel());
        ViewBag.MemberId = q.ToSelectList(memberId);
    }

    private async Task PopulateVehiclesDropDownListAsync(string? vehicleId = null)
    {
        var q = (await masterDataService.GetVehiclesAsync()).Select(x => x.ToVehicleViewModel());
        ViewBag.VehicleId = q.ToSelectList(vehicleId);
    }

    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = "ForumChallengeAdminRole")]
    public async Task<IActionResult> Create(
        [Bind("TimeString,Post,Vehicle,Member,ForumChallenge")] ForumChallengeRunViewModel run)
    {
        if (ModelState.IsValid && !string.IsNullOrEmpty(run.TimeString))
        {
            run.Time = run.TimeString.FromTimestringToInt();
            await forumChallengeService.AddForumChallengeRunAsync(run.ToDto());
            return RedirectToAction(nameof(Index));
        }

        await PopulateChallengesDropDownListAsync(run.ForumChallengeRunId);
        await PopulateMembersDropDownListAsync(run.Member.MemberId);
        await PopulateVehiclesDropDownListAsync(run.Vehicle.VehicleId);
        return View(run);
    }

    public async Task<IActionResult> Edit(string? id)
    {
        if (id == null)
            return NotFound();

        var run = await forumChallengeService.GetForumChallengeRunAsync(id);
        if (run == null)
            return NotFound();
        await PopulateChallengesDropDownListAsync(run.ForumChallenge.Id);
        await PopulateMembersDropDownListAsync(run.Member.Id);
        await PopulateVehiclesDropDownListAsync(run.Vehicle.Id);

        return View(run.ToForumChallengeRunViewModel());
    }

    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = "ForumChallengeAdminRole")]
    public async Task<IActionResult> Edit(string id,
        [Bind("ForumChallengeRunId,TimeString,Vehicle,Post,Member,ForumChallenge, Deleted")]
        ForumChallengeRunViewModel run)
    {
        if (id != run.ForumChallengeRunId)
            return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                run.Time = run.TimeString.FromTimestringToInt();
                await forumChallengeService.UpdateForumChallengeRunAsync(run.ToDto());
            }
            catch (Exception)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }

        return View(run);
    }

    public async Task<IActionResult> Delete(string? id)
    {
        if (id == null)
            return NotFound();

        var run = await forumChallengeService.GetForumChallengeRunAsync(id);
        if (run == null)
            return NotFound();

        return View(run.ToForumChallengeRunViewModel());
    }

    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = "ForumChallengeAdminRole")]
    public async Task<IActionResult> DeleteConfirmed(string id)
    {
        await forumChallengeService.DeleteForumChallengeRunAsync(id);
        return RedirectToAction(nameof(Index));
    }
}