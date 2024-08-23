using A8Forum.Mappers;
using A8Forum.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Services;

namespace A8Forum.Controllers
{
    [Authorize(Policy = "AdminRole")]
    public class MembersController : Controller
    {
        private readonly IMasterDataService _masterDataService;

        public MembersController(IMasterDataService masterDataService)
        {
            _masterDataService = masterDataService;
        }

        // GET: Members
        public async Task<IActionResult> Index()
        {
            var m = await _masterDataService.GetMembersAsync();
            return View(m.ToList().Select(x => x.ToMemberViewModel()));
        }

        public async Task<IActionResult> Details(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var member = await _masterDataService.GetMemberAsync(id);

            if (member == null)
            {
                return NotFound();
            }

            return View(member.ToMemberViewModel());
        }

        public IActionResult Create()
        {
            return View();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MemberId,MemberName, MemberDisplayName, Guest")] MemberViewModel member)
        {
            if (ModelState.IsValid)
            {
                await _masterDataService.AddMemberAsync(member.ToDto());
                return RedirectToAction(nameof(Index));
            }
            return View(member);
        }

        public async Task<IActionResult> Edit(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var member = await _masterDataService.GetMemberAsync(id);
            if (member == null)
            {
                return NotFound();
            }
            return View(member.ToMemberViewModel());
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("MemberId,MemberName," +
            "MemberDisplayName, Guest")] MemberViewModel member)
        {
            if (id != member.MemberId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _masterDataService.UpdateMemberAsync(member.ToDto());
                }
                catch (Exception ex)
                {
                    return NotFound();
                }
                return RedirectToAction(nameof(Index));
            }
            return View(member);
        }

        public async Task<IActionResult> Delete(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var member = await _masterDataService.GetMemberAsync(id);
            if (member == null)
            {
                return NotFound();
            }

            return View(member.ToMemberViewModel());
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var member = await _masterDataService.GetMemberAsync(id);
            if (member != null)
            {
                _masterDataService.DeleteMemberAsync(id);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}