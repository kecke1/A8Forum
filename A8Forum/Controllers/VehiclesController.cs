﻿using A8Forum.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Services;

namespace A8Forum.Controllers
{
    [Authorize]
    public class VehiclesController : Controller
    {
        private readonly IMasterDataService _masterDataService;

        public VehiclesController(IMasterDataService masterDataService)
        {
            _masterDataService = masterDataService;
        }

        public async Task<IActionResult> Import()
        {
            // await _vehicleService.Import();

            return RedirectToAction(nameof(Index));
        }

        // GET: Vehicles
        public async Task<IActionResult> Index()
        {
            var v = await _masterDataService.GetVehiclesAsync();
            return View(v.ToList().Select(x => x.ToVehicleViewModel()));
        }

        // GET: Vehicles/Details/5
        public async Task<IActionResult> Details(string id)
        {
            var vehicle = await _masterDataService.GetVehicleAsync(id);
            if (vehicle == null)
            {
                return NotFound();
            }

            return View(vehicle.ToVehicleViewModel());
        }

        // GET: Vehicles/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Vehicles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminRole")]
        public async Task<IActionResult> Create([Bind("Name,Url,MaxRank, ShortName, Keyword")] ViewModels.VehicleViewModel vehicle)
        {
            if (ModelState.IsValid)
            {
                await _masterDataService.AddVehicleAsync(vehicle.ToDto());
                return RedirectToAction(nameof(Index));
            }
            return View(vehicle);
        }

        // GET: Vehicles/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            var vehicle = await _masterDataService.GetVehicleAsync(id);
            if (vehicle == null)
            {
                return NotFound();
            }
            return View(vehicle.ToVehicleViewModel());
        }

        // POST: Vehicles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminRole")]
        public async Task<IActionResult> Edit(string id, [Bind("VehicleId,Name,Url,MaxRank, ShortName, Keyword")] ViewModels.VehicleViewModel vehicle)
        {
            if (id != vehicle.VehicleId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _masterDataService.UpdateVehicleAsync(vehicle.ToDto());
                return RedirectToAction(nameof(Index));
            }
            return View(vehicle);
        }

        // GET: Vehicles/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            var vehicle = await _masterDataService.GetVehicleAsync(id);
            if (vehicle == null)
            {
                return NotFound();
            }

            return View(vehicle.ToVehicleViewModel());
        }

        // POST: Vehicles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminRole")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            await _masterDataService.DeleteVehicleAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}