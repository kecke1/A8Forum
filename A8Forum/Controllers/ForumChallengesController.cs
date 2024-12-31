using A8Forum.Extensions;
using A8Forum.Mappers;
using A8Forum.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Services;

namespace A8Forum.Controllers;

[Authorize]
public class ForumChallengesController(IMasterDataService masterDataService,
        IForumChallengeService forumChallengeService)
    : Controller
{
    public async Task<IActionResult> Index(string? trackId = null, DateTime? InsertDateFrom = null,
        DateTime? InsertDateTo = null)
    {
        await PopulateTracksDropDownListAsync(trackId);
        ViewData["InsertDateFrom"] = InsertDateFrom ?? DateTime.Now.AddYears(-1);
        ViewData["InsertDateTo"] = InsertDateTo ?? DateTime.Now;

        var query = (await forumChallengeService
                .GetForumChallengesAsync((DateTime)ViewData["InsertDateFrom"], (DateTime)ViewData["InsertDateTo"]))
            .ToList();

        if (!string.IsNullOrEmpty(trackId) && trackId != "-1")
            query = query.Where(x => x.Track.Id == trackId).ToList();

        return View(query
            .Select(x => x.ToForumChallengeViewModel())
            .OrderByDescending(x => x.EndDate));
    }

    public async Task<IActionResult> Details(string? id)
    {
        if (id == null)
            return NotFound();

        var challenge = await forumChallengeService.GetForumChallengeAsync(id);
        if (challenge == null)
            return NotFound();
        return View(challenge.ToForumChallengeViewModel());
    }

    public async Task<IActionResult> Create()
    {
        await PopulateTracksDropDownListAsync();
        await PopulateSeasonsDropDownListAsync();
        return View();
    }

    private async Task PopulateSeasonsDropDownListAsync(string? seasonId = null)
    {
        var q = (await masterDataService.GetSeasonsAsync())
            .Select(x => x.ToSeasonViewModel());
        ViewBag.SeasonId = q.ToSelectList(seasonId);
    }

    private async Task PopulateTracksDropDownListAsync(string? trackId = null)
    {
        var q = (await masterDataService.GetTracksAsync())
            .Select(x => x.ToTrackViewModel())
            .OrderBy(x => x.TrackName);
        ViewBag.TrackId = q.ToSelectList(trackId);
    }

    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = "ForumChallengeAdminRole")]
    public async Task<IActionResult> Create(
        [Bind("StartDate,EndDate,Rules,Post,MaxRank, Track,Season,CustomTitle")]
        ForumChallengeViewModel challenge)
    {
        if (ModelState.IsValid)
        {
            await forumChallengeService.AddForumChallengeAsync(challenge.ToDto());
            return RedirectToAction(nameof(Index));
        }

        await PopulateSeasonsDropDownListAsync(challenge.Season.SeasonId);
        await PopulateTracksDropDownListAsync(challenge.Track.TrackId);
        return View(challenge);
    }

    public async Task<IActionResult> Edit(string? id)
    {
        if (id == null)
            return NotFound();

        var challenge = await forumChallengeService.GetForumChallengeAsync(id);
        if (challenge == null)
            return NotFound();

        await PopulateSeasonsDropDownListAsync(challenge.Season.Id);
        await PopulateTracksDropDownListAsync(challenge.Track.Id);
        return View(challenge.ToForumChallengeViewModel());
    }

    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = "ForumChallengeAdminRole")]
    public async Task<IActionResult> Edit(string id,
        [Bind("ForumChallengeId,StartDate,EndDate,Rules,Post,MaxRank, Track, Season,CustomTitle")]
        ForumChallengeViewModel challenge)
    {
        if (id != challenge.ForumChallengeId)
            return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                await forumChallengeService.UpdateForumChallengeAsync(challenge.ToDto());
            }
            catch (Exception)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }

        return View(challenge);
    }

    public async Task<IActionResult> Delete(string? id)
    {
        if (id == null)
            return NotFound();

        var challenge = await forumChallengeService.GetForumChallengeAsync(id);
        if (challenge == null)
            return NotFound();

        return View(challenge.ToForumChallengeViewModel());
    }

    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = "ForumChallengeAdminRole")]
    public async Task<IActionResult> DeleteConfirmed(string id)
    {
        forumChallengeService.DeleteForumChallengeAsync(id);
        return RedirectToAction(nameof(Index));
    }
}