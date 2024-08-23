﻿using A8Forum.Extensions;
using A8Forum.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Shared.Extensions;
using Shared.Services;
using System.Globalization;

namespace A8Forum.Controllers
{
    [Authorize]
    public class GiftLinksController : Controller
    {
        private readonly IGiftLinkService _giftLinkService;
        private readonly IMasterDataService _masterDataService;

        public GiftLinksController(IGiftLinkService giftLinkService,
            IMasterDataService masterDataService)
        {
            _giftLinkService = giftLinkService;
            _masterDataService = masterDataService;
        }

        // GET: GiftLinks
        public async Task<IActionResult> Index(string? monthId = null)
        {
            var mId = monthId ?? DateTime.Now.ToString("yyyy-MM");

            PopulateMonthsDropDownList(mId);

            var filteredMont = DateTime.ParseExact(mId, "yyyy-MM", CultureInfo.InstalledUICulture, DateTimeStyles.None);
            var giftLinkList = (await _giftLinkService.GetGiftLinksAsync())
                .Where(x => !x.Deleted)
                .ToList();
            var giftLinkProviders = await _giftLinkService.GetGiftLinkProvidersAsync();
            var currentMontGiftLinks = giftLinkList.Where(x => x.Month == filteredMont);
            ViewBag.GiftLinkList = giftLinkList.Any() ? _giftLinkService.CreateGiftLinkList(currentMontGiftLinks) : "";
            ViewBag.GiftLinkListHtml = ((string)ViewBag.GiftLinkList).ToHtml();

            var giftLinkProviderList = _giftLinkService.GetActiveGiftLinkProviders(filteredMont, giftLinkProviders, giftLinkList);

            ViewBag.GiftLinkProviderList = _giftLinkService.CreateGiftLinkProviderList(giftLinkProviderList, filteredMont);
            // ViewBag.GiftLinkListHtml = _giftLinkService.CreateGiftLinkProviderList(giftLinkProviderList, filteredMont);

            return View(currentMontGiftLinks.Select(x => x.ToGiftLinkViewModel()));
        }

        private async void PopulateMonthsDropDownList(string? mId)
        {
            var q = (await _giftLinkService.GetGiftLinksAsync())
                .Where(x => !x.Deleted)
                .OrderBy(x => x.Month)
                .Select(x => x.Month.ToString("yyyy-MM"))
                .Distinct()
                .ToList();

            q = q.Append($"{DateTime.Now:yyyy-MM}").Distinct().ToList();

            ViewBag.MonthId = new SelectList(q, mId ?? q.First());
        }

        // GET: GiftLinks/Details/5
        public async Task<IActionResult> Details(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var giftLink = await _giftLinkService.GetGiftLinkAsync(id);
            if (giftLink == null)
            {
                return NotFound();
            }

            return View(giftLink.ToGiftLinkViewModel());
        }

        // GET: GiftLinks/Create
        public async Task<IActionResult> Create()
        {
            // PopulateRacersDropDownList();
            //PopulatGiftLinkProvidersDropDownList();

            var members = (await _masterDataService.GetMembersAsync()).Select(x => x.ToMemberViewModel()).ToList();
            ViewData["MemberId"] = members.ToSelectList(null);
            ViewData["GiftLinkProviderId"] = await GetProvidersSelectList();
            return View();
        }

        private async Task<SelectList> GetProvidersSelectList(string? selectedValue = null)
        {
            var providers = (await _giftLinkService.GetGiftLinkProvidersAsync()).ToList();
            return new SelectList(providers.Where(x => !x.Deleted), "Id", "Name", selectedValue);
        }

        // POST: GiftLinks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "GiftLinkRole")]
        public async Task<IActionResult> Create([Bind("GiftLinkId,Url,GiftLinkProvider,SubmitedBy,Deleted,Month,Notes, IgnoreGiftLinkValidation")] ViewModels.GiftLinkViewModel giftLink)
        {
            if (!giftLink.Url.StartsWith("http"))
            {
                giftLink.Url = $"https://asphaltairborne.page.link/{giftLink.Url.Trim()}";
            }

            if (ModelState.IsValid)
            {
                var giftLinks = (await _giftLinkService.GetGiftLinksAsync()).ToList();

                var duplicates = giftLinks.Where(x => x.Url == giftLink.Url && !x.Deleted)
                    .OrderByDescending(x => x.Month)
                    .FirstOrDefault();

                if (duplicates == null || giftLink.IgnoreDuplicateValidation)
                {
                    await _giftLinkService.AddGiftLinkAsync(giftLink.ToDto());
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError("Url", $"Url already exists in {duplicates.Month:yyyy-MM}");
                ViewData["Url"] = giftLink.Url;
            }
            ViewData["GiftLinkProviderId"] = await GetProvidersSelectList(giftLink.GiftLinkProvider.GiftLinkProviderId);
            var members = (await _masterDataService.GetMembersAsync()).Select(x => x.ToMemberViewModel()).ToList();
            ViewData["MemberId"] = members.ToSelectList(giftLink.SubmitedBy.MemberId);
            return View(giftLink);
        }

        // GET: GiftLinks/Edit/5
        public async Task<IActionResult> Edit(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var giftLink = await _giftLinkService.GetGiftLinkAsync(id);
            if (giftLink == null)
            {
                return NotFound();
            }
            var members = (await _masterDataService.GetMembersAsync()).Select(x => x.ToMemberViewModel()).ToList();
            ViewData["MemberId"] = members.ToSelectList(giftLink.Member.Id);
            ViewData["GiftLinkProviderId"] = await GetProvidersSelectList(giftLink.GiftLinkProvider.Id);
            return View(giftLink.ToGiftLinkViewModel());
        }

        // POST: GiftLinks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "GiftLinkRole")]
        public async Task<IActionResult> Edit(string id, [Bind("GiftLinkId,Url,GiftLinkProvider,SubmitedBy,Deleted,Idate,Month,Notes")] ViewModels.GiftLinkViewModel giftLink)
        {
            if (id != giftLink.GiftLinkId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _giftLinkService.UpdateGiftLinkAsync(giftLink.ToDto());
                }
                catch (Exception)
                {
                    return NotFound();
                }
                return RedirectToAction(nameof(Index));
            }
            var members = (await _masterDataService.GetMembersAsync()).Select(x => x.ToMemberViewModel()).ToList();
            ViewData["MemberId"] = members.ToSelectList(giftLink.SubmitedBy.MemberId);
            ViewData["GiftLinkProviderId"] = await GetProvidersSelectList(giftLink.GiftLinkProvider.GiftLinkProviderId);

            return View(giftLink);
        }

        // GET: GiftLinks/Delete/5
        public async Task<IActionResult> Delete(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var giftLink = await _giftLinkService.GetGiftLinkAsync(id);
            if (giftLink == null)
            {
                return NotFound();
            }

            return View(giftLink.ToGiftLinkViewModel());
        }

        // POST: GiftLinks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "GiftLinkRole")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            await _giftLinkService.DeleteGiftLinkAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}