using A8Forum.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Services;

namespace A8Forum.Controllers
{
    [Authorize]
    public class CareerRacesController : Controller
    {
        private readonly IMasterDataService _masterDataService;

        public CareerRacesController(IMasterDataService masterDataService)
        {
            _masterDataService = masterDataService;
        }

        // GET: CareerRaces
        public async Task<IActionResult> Index()
        {
            var c = await _masterDataService.GetCareerRacesAsync();
            return View(c.Select(x => x.ToCareerRaceViewModel()));
        }

        // GET: CareerRaces/Details/5
        public async Task<IActionResult> Details(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var careerRace = await _masterDataService.GetCareerRaceAsync(id);
            if (careerRace == null)
            {
                return NotFound();
            }

            return View(careerRace);
        }

        // GET: CareerRaces/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CareerRaces/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Policy = "AdminRole")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CareerRaceType,TrackId,Class,SeasonId,Limitations,LimitationsDescription")] ViewModels.CareerRaceViewModel careerRace)
        {
            if (ModelState.IsValid)
            {
                await _masterDataService.AddCareerRaceAsync(careerRace.ToDto());
                return RedirectToAction(nameof(Index));
            }
            return View(careerRace);
        }

        // GET: CareerRaces/Edit/5
        public async Task<IActionResult> Edit(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var careerRace = await _masterDataService.GetCareerRaceAsync(id);
            if (careerRace == null)
            {
                return NotFound();
            }
            return View(careerRace.ToCareerRaceViewModel());
        }

        // POST: CareerRaces/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminRole")]
        public async Task<IActionResult> Edit(string id, [Bind("CareerRaceId,CareerRaceType,TrackId,Class,SeasonId,Limitations,LimitationsDescription")] ViewModels.CareerRaceViewModel careerRace)
        {
            if (id != careerRace.CareerRaceId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _masterDataService.UpdateCareerRaceAsync(careerRace.ToDto());
                }
                catch (Exception)
                {
                    return NotFound();
                }
                return RedirectToAction(nameof(Index));
            }
            return View(careerRace);
        }

        // GET: CareerRaces/Delete/5
        public async Task<IActionResult> Delete(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var careerRace = await _masterDataService.GetCareerRaceAsync(id);
            if (careerRace == null)
            {
                return NotFound();
            }

            return View(careerRace.ToCareerRaceViewModel());
        }

        // POST: CareerRaces/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminRole")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            _masterDataService.DeleteCareerRaceAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}