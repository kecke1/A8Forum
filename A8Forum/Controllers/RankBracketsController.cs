﻿using A8Forum.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Services;

namespace A8Forum.Controllers
{
    [Authorize]
    public class RankBracketsController : Controller
    {
        private readonly IMasterDataService _masterDataService;

        public RankBracketsController(IMasterDataService masterDataService)
        {
            _masterDataService = masterDataService;
        }

        // GET: RankBrackets
        public async Task<IActionResult> Index()
        {
            var r = await _masterDataService.GetRankBracketsAsync();
            return View(r.Select(x => x.ToRankBracketViewModel()));
        }

        // GET: RankBrackets/Details/5
        public async Task<IActionResult> Details(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var r = await _masterDataService.GetRankBracketAsync(id);
            if (r == null)
            {
                return NotFound();
            }

            return View(r.ToRankBracketViewModel());
        }

        // GET: RankBrackets/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: RankBrackets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminRole")]
        public async Task<IActionResult> Create([Bind("MinRank,MaxRank,Class")] ViewModels.RankBracketViewModel rankBracket)
        {
            if (ModelState.IsValid)
            {
                _masterDataService.AddRankBracketAsync(rankBracket.ToDto());
                return RedirectToAction(nameof(Index));
            }
            return View(rankBracket);
        }

        // GET: RankBrackets/Edit/5
        public async Task<IActionResult> Edit(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rankBracket = await _masterDataService.GetRankBracketAsync(id);
            if (rankBracket == null)
            {
                return NotFound();
            }
            return View(rankBracket.ToRankBracketViewModel());
        }

        // POST: RankBrackets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminRole")]
        public async Task<IActionResult> Edit(string id, [Bind("MinRank,MaxRank,Class, RankBracketId")] ViewModels.RankBracketViewModel rankBracket)
        {
            if (id != rankBracket.RankBracketId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _masterDataService.UpdateRankBracketAsync(rankBracket.ToDto());
                }
                catch (Exception)
                {
                    return NotFound();
                }
                return RedirectToAction(nameof(Index));
            }
            return View(rankBracket);
        }

        // GET: RankBrackets/Delete/5
        public async Task<IActionResult> Delete(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rankBracket = await _masterDataService.GetRankBracketAsync(id);
            if (rankBracket == null)
            {
                return NotFound();
            }

            return View(rankBracket.ToRankBracketViewModel());
        }

        // POST: RankBrackets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminRole")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            _masterDataService.DeleteRankBracketAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}