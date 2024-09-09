using A8Forum.Mappers;
using A8Forum.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Services;

namespace A8Forum.Controllers;

[Authorize]
public class SeasonsController(IMasterDataService service) : Controller
{
    // GET: Seasons
    public async Task<IActionResult> Index()
    {
        var s = await service.GetSeasonsAsync();
        return View(s.Select(x => x.ToSeasonViewModel()));
    }

    // GET: Seasons/Details/5
    public async Task<IActionResult> Details(string? id)
    {
        if (id == null)
            return NotFound();

        var season = await service.GetSeasonAsync(id);
        if (season == null)
            return NotFound();

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
    public async Task<IActionResult> Create([Bind("SeasonId,SeasonName")] SeasonViewModel season)
    {
        if (ModelState.IsValid)
        {
            await service.AddSeasonAsync(season.ToDto());
            return RedirectToAction(nameof(Index));
        }

        return View(season);
    }

    // GET: Seasons/Edit/5
    public async Task<IActionResult> Edit(string? id)
    {
        if (id == null)
            return NotFound();

        var season = await service.GetSeasonAsync(id);
        if (season == null)
            return NotFound();
        return View(season.ToSeasonViewModel());
    }

    // POST: Seasons/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = "AdminRole")]
    public async Task<IActionResult> Edit(string id, [Bind("SeasonId,SeasonName")] SeasonViewModel season)
    {
        if (id != season.SeasonId)
            return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                await service.UpdateSeasonAsync(season.ToDto());
            }
            catch (Exception)
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
            return NotFound();

        var season = await service.GetSeasonAsync(id);
        if (season == null)
            return NotFound();

        return View(season.ToSeasonViewModel());
    }

    // POST: Seasons/Delete/5
    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = "AdminRole")]
    public async Task<IActionResult> DeleteConfirmed(string id)
    {
        await service.DeleteSeasonAsync(id);
        return RedirectToAction(nameof(Index));
    }
}