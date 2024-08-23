using A8Forum.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Services;

namespace A8Forum.Controllers
{
    [Authorize]
    public class GiftLinkProvidersController : Controller
    {
        private readonly IGiftLinkService _giftLinkService;

        public GiftLinkProvidersController(IGiftLinkService giftLinkService)
        {
            _giftLinkService = giftLinkService;
        }

        // GET: GiftLinkProviders
        public async Task<IActionResult> Index()
        {
            return View((await _giftLinkService.GetGiftLinkProvidersAsync())
                .Select(x => x.ToGiftLinkProviderViewModel()).ToList());
        }

        // GET: GiftLinkProviders/Details/5
        public async Task<IActionResult> Details(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var giftLinkProvider = await _giftLinkService.GetGiftLinkProviderAsync(id);
            if (giftLinkProvider == null)
            {
                return NotFound();
            }

            return View(giftLinkProvider.ToGiftLinkProviderViewModel());
        }

        // GET: GiftLinkProviders/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: GiftLinkProviders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "GiftLinkRole")]
        public async Task<IActionResult> Create([Bind("GiftLinkProviderId,Name,Url,Deleted,Display")] ViewModels.GiftLinkProvider giftLinkProvider)
        {
            if (ModelState.IsValid)
            {
                await _giftLinkService.AddGiftLinkProviderAsync(giftLinkProvider.ToDto());
                return RedirectToAction(nameof(Index));
            }
            return View(giftLinkProvider);
        }

        // GET: GiftLinkProviders/Edit/5
        public async Task<IActionResult> Edit(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var giftLinkProvider = await _giftLinkService.GetGiftLinkProviderAsync(id);
            if (giftLinkProvider == null)
            {
                return NotFound();
            }
            return View(giftLinkProvider.ToGiftLinkProviderViewModel());
        }

        // POST: GiftLinkProviders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "GiftLinkRole")]
        public async Task<IActionResult> Edit(string id, [Bind("GiftLinkProviderId,Name,Url,Deleted,Hide")] ViewModels.GiftLinkProvider giftLinkProvider)
        {
            if (id != giftLinkProvider.GiftLinkProviderId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _giftLinkService.UpdateGiftLinkProviderAsync(giftLinkProvider.ToDto());
                }
                catch (Exception)
                {
                    return NotFound();
                }
                return RedirectToAction(nameof(Index));
            }
            return View(giftLinkProvider);
        }

        // GET: GiftLinkProviders/Delete/5
        public async Task<IActionResult> Delete(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var giftLinkProvider = await _giftLinkService.GetGiftLinkProviderAsync(id);
            if (giftLinkProvider == null)
            {
                return NotFound();
            }

            return View(giftLinkProvider.ToGiftLinkProviderViewModel());
        }

        // POST: GiftLinkProviders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "GiftLinkRole")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            await _giftLinkService.DeleteGiftLinkProviderAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}