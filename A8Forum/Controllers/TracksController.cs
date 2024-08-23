using A8Forum.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Services;

namespace A8Forum.Controllers
{
    [Authorize]
    public class TracksController : Controller
    {
        private readonly IMasterDataService _masterDataService;

        public TracksController(IMasterDataService masterDataService)
        {
            _masterDataService = masterDataService;
        }

        // GET: Tracks
        public async Task<IActionResult> Index()
        {
            var t = await _masterDataService.GetTracksAsync();
            return View(t.Select(x => x.ToTrackViewModel()));
        }

        // GET: Tracks/Details/5
        public async Task<IActionResult> Details(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var track = await _masterDataService.GetTrackAsync(id);
            if (track == null)
            {
                return NotFound();
            }

            return View(track.ToTrackViewModel());
        }

        // GET: Tracks/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Tracks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminRole")]
        public async Task<IActionResult> Create([Bind("TrackId,TrackName")] ViewModels.TrackViewModel track)
        {
            if (ModelState.IsValid)
            {
                await _masterDataService.AddTrackAsync(track.ToDto());
                return RedirectToAction(nameof(Index));
            }
            return View(track);
        }

        // GET: Tracks/Edit/5
        public async Task<IActionResult> Edit(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var track = await _masterDataService.GetTrackAsync(id);
            if (track == null)
            {
                return NotFound();
            }
            return View(track.ToTrackViewModel());
        }

        // POST: Tracks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminRole")]
        public async Task<IActionResult> Edit(string id, [Bind("TrackId,TrackName")] ViewModels.TrackViewModel track)
        {
            if (id != track.TrackId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _masterDataService.UpdateTrackAsync(track.ToDto());
                }
                catch (Exception)
                {
                    return NotFound();
                }
                return RedirectToAction(nameof(Index));
            }
            return View(track);
        }

        // GET: Tracks/Delete/5
        public async Task<IActionResult> Delete(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var track = await _masterDataService.GetTrackAsync(id);
            if (track == null)
            {
                return NotFound();
            }

            return View(track.ToTrackViewModel());
        }

        // POST: Tracks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminRole")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            _masterDataService.DeleteTrackAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}