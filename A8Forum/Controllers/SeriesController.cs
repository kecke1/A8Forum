using A8Forum.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Services;

namespace A8Forum.Controllers
{
    [Authorize]
    public class SeriesController : Controller
    {
        private readonly IMasterDataService _masterDataService;
        private readonly IForumChallengeService _forumChallengeService;

        public SeriesController(IMasterDataService masterDataService,
            IForumChallengeService forumChallengeService)
        {
            _forumChallengeService = forumChallengeService;
            _masterDataService = masterDataService;
        }

        // GET: Series
        public async Task<IActionResult> Index()
        {
            var s = await _forumChallengeService.GetSeriesAsync(DateTime.Now.AddYears(-1), DateTime.Now);

            return View(s.Select(x => x.ToSeriesViewModel("", "")));
        }

        // GET: Series/Details/5
        public async Task<IActionResult> Details(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var series = await _forumChallengeService.GetSeriesAsync(id);
            if (series == null)
            {
                return NotFound();
            }
            var lb = await _forumChallengeService.GetSeriesLeaderboardAsync(id);

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
        public async Task<IActionResult> Create([Bind("SeriesId,StartDate,EndDate,NumberOfRaces")] ViewModels.SeriesViewModel series)
        {
            if (ModelState.IsValid)
            {
                await _forumChallengeService.AddSeriesAsync(series.ToDto());
                return RedirectToAction(nameof(Index));
            }
            return View(series);
        }

        // GET: Series/Edit/5
        public async Task<IActionResult> Edit(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var series = await _forumChallengeService.GetSeriesAsync(id);
            if (series == null)
            {
                return NotFound();
            }
            return View(series.ToSeriesViewModel("", ""));
        }

        // POST: Series/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "ForumChallengeAdminRole")]
        public async Task<IActionResult> Edit(string id, [Bind("SeriesId,StartDate,EndDate,NumberOfRaces")] ViewModels.SeriesViewModel series)
        {
            if (id != series.SeriesId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _forumChallengeService.UpdateSeriesAsync(series.ToDto());
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
            {
                return NotFound();
            }

            var series = await _forumChallengeService.GetSeriesAsync(id);
            if (series == null)
            {
                return NotFound();
            }

            return View(series.ToSeriesViewModel("", ""));
        }

        // POST: Series/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "ForumChallengeAdminRole")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            await _forumChallengeService.DeleteSeriesAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}