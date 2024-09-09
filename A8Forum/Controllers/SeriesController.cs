using A8Forum.Mappers;
using A8Forum.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Services;

namespace A8Forum.Controllers;

[Authorize]
public class SeriesController(IMasterDataService masterDataService,
        IForumChallengeService forumChallengeService)
    : Controller
{
    private readonly IMasterDataService _masterDataService = masterDataService;

    // GET: Series
    public async Task<IActionResult> Index()
    {
        var s = await forumChallengeService.GetSeriesAsync(DateTime.Now.AddYears(-1), DateTime.Now);

        return View(s.Select(x => x.ToSeriesViewModel("", "")));
    }

    // GET: Series/Details/5
    public async Task<IActionResult> Details(string? id)
    {
        if (id == null)
            return NotFound();

        var series = await forumChallengeService.GetSeriesAsync(id);
        if (series == null)
            return NotFound();
        var lb = await forumChallengeService.GetSeriesLeaderboardAsync(id);

        return View(series.ToSeriesViewModel(lb, lb));
    }

    // GET: Series/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Series/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = "ForumChallengeAdminRole")]
    public async Task<IActionResult> Create([Bind("SeriesId,StartDate,EndDate,NumberOfRaces")] SeriesViewModel series)
    {
        if (ModelState.IsValid)
        {
            await forumChallengeService.AddSeriesAsync(series.ToDto());
            return RedirectToAction(nameof(Index));
        }

        return View(series);
    }

    // GET: Series/Edit/5
    public async Task<IActionResult> Edit(string? id)
    {
        if (id == null)
            return NotFound();

        var series = await forumChallengeService.GetSeriesAsync(id);
        if (series == null)
            return NotFound();
        return View(series.ToSeriesViewModel("", ""));
    }

    // POST: Series/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = "ForumChallengeAdminRole")]
    public async Task<IActionResult> Edit(string id,
        [Bind("SeriesId,StartDate,EndDate,NumberOfRaces")] SeriesViewModel series)
    {
        if (id != series.SeriesId)
            return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                forumChallengeService.UpdateSeriesAsync(series.ToDto());
            }
            catch (Exception)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }

        return View(series);
    }

    // GET: Series/Delete/5
    public async Task<IActionResult> Delete(string? id)
    {
        if (id == null)
            return NotFound();

        var series = await forumChallengeService.GetSeriesAsync(id);
        if (series == null)
            return NotFound();

        return View(series.ToSeriesViewModel("", ""));
    }

    // POST: Series/Delete/5
    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = "ForumChallengeAdminRole")]
    public async Task<IActionResult> DeleteConfirmed(string id)
    {
        await forumChallengeService.DeleteSeriesAsync(id);
        return RedirectToAction(nameof(Index));
    }
}