using A8Forum.Mappers;
using A8Forum.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Services;

namespace A8Forum.Controllers;

[Authorize]
public class TracksController(IMasterDataService masterDataService) : Controller
{
    // GET: Tracks
    public async Task<IActionResult> Index()
    {
        var t = await masterDataService.GetTracksAsync(true, true);
        return View(t.Select(x => x.ToTrackViewModel()));
    }

    // GET: Tracks/Details/5
    public async Task<IActionResult> Details(string? id)
    {
        if (id == null)
            return NotFound();

        var track = await masterDataService.GetTrackAsync(id);
        if (track == null)
            return NotFound();

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
    public async Task<IActionResult> Create([Bind("TrackId,TrackName, Sprint, Order")] TrackViewModel track)
    {
        if (ModelState.IsValid)
        {
            await masterDataService.AddTrackAsync(track.ToDto());
            return RedirectToAction(nameof(Index));
        }

        return View(track);
    }

    // GET: Tracks/Edit/5
    public async Task<IActionResult> Edit(string? id)
    {
        if (id == null)
            return NotFound();

        var track = await masterDataService.GetTrackAsync(id);
        if (track == null)
            return NotFound();
        return View(track.ToTrackViewModel());
    }

    // POST: Tracks/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = "AdminRole")]
    public async Task<IActionResult> Edit(string id, [Bind("TrackId,TrackName, Sprint, Order")] TrackViewModel track)
    {
        if (id != track.TrackId)
            return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                await masterDataService.UpdateTrackAsync(track.ToDto());
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
            return NotFound();

        var track = await masterDataService.GetTrackAsync(id);
        if (track == null)
            return NotFound();

        return View(track.ToTrackViewModel());
    }

    // POST: Tracks/Delete/5
    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = "AdminRole")]
    public async Task<IActionResult> DeleteConfirmed(string id)
    {
        masterDataService.DeleteTrackAsync(id);
        return RedirectToAction(nameof(Index));
    }


    [HttpGet("Find")]
        public async Task<IActionResult> Find([FromQuery] string? term, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
          /*  var results = 
            // Select2 expects { results: [{id,text}], pagination: { more: bool } }
            return Ok(new
            {
                results = results.Items.Select(t => new { id = t.Id, text = t.Name }),
                pagination = new { more = results.HasMore }
            });
          */
          return Ok();
        }


    [HttpGet("Byids")]
    public async Task<IActionResult> GetByIds([FromQuery] List<int> ids)
    {
     //   var tracks = await _service.GetByIdsAsync(ids);
      //  return Ok(tracks.Select(t => new { id = t.Id, text = t.Name }));
      return Ok();
    }

}