using A8Forum.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Services;

namespace A8Forum.Controllers
{
    [Authorize]
    public class SeasonsController : Controller
    {
        private readonly IMasterDataService _masterDataService;

        public SeasonsController(IMasterDataService service)
        {
            _masterDataService = service;
        }

        // GET: Seasons
        public async Task<IActionResult> Index()
        {
            var s = await _masterDataService.GetSeasonsAsync();
            return View(s.Select(x => x.ToSeasonViewModel()));
        }

        // GET: Seasons/Details/5
        public async Task<IActionResult> Details(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var season = await _masterDataService.GetSeasonAsync(id);
            if (season == null)
            {
                return NotFound();
            }

            return View(season.ToSeasonViewModel());
        }

        // GET: Seasons/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Seasons/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminRole")]
        public async Task<IActionResult> Create([Bind("SeasonId,SeasonName")] ViewModels.SeasonViewModel season)
        {
            if (ModelState.IsValid)
            {
                await _masterDataService.AddSeasonAsync(season.ToDto());
                return RedirectToAction(nameof(Index));
            }
            return View(season);
        }

        // GET: Seasons/Edit/5
        public async Task<IActionResult> Edit(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var season = await _masterDataService.GetSeasonAsync(id);
            if (season == null)
            {
                return NotFound();
            }
            return View(season.ToSeasonViewModel());
        }

        // POST: Seasons/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminRole")]
        public async Task<IActionResult> Edit(string id, [Bind("SeasonId,SeasonName")] ViewModels.SeasonViewModel season)
        {
            if (id != season.SeasonId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _masterDataService.UpdateSeasonAsync(season.ToDto());
                }
                catch (Exception ex)
                {
                    return NotFound();
                }
                return RedirectToAction(nameof(Index));
            }
            return View(season);
        }

        // GET: Seasons/Delete/5
        public async Task<IActionResult> Delete(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var season = await _masterDataService.GetSeasonAsync(id);
            if (season == null)
            {
                return NotFound();
            }

            return View(season.ToSeasonViewModel());
        }

        // POST: Seasons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminRole")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            await _masterDataService.DeleteSeasonAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}