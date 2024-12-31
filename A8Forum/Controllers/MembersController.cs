using A8Forum.Mappers;
using A8Forum.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Services;

namespace A8Forum.Controllers;

[Authorize]
public class MembersController(IMasterDataService masterDataService) : Controller
{
    // GET: Members
    public async Task<IActionResult> Index()
    {
        var m = await masterDataService.GetMembersAsync();
        return View(m.ToList().Select(x => x.ToMemberViewModel()));
    }

    public async Task<IActionResult> Details(string? id)
    {
        if (id == null)
            return NotFound();

        var member = await masterDataService.GetMemberAsync(id);

        if (member == null)
            return NotFound();

        return View(member.ToMemberViewModel());
    }

    public IActionResult Create()
    {
        return View();
    }

    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [Authorize(Policy = "AdminRole")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind("MemberId,MemberName, MemberDisplayName, Guest")]
        MemberViewModel member)
    {
        if (ModelState.IsValid)
        {
            await masterDataService.AddMemberAsync(member.ToDto());
            return RedirectToAction(nameof(Index));
        }

        return View(member);
    }

    public async Task<IActionResult> Edit(string? id)
    {
        if (id == null)
            return NotFound();

        var member = await masterDataService.GetMemberAsync(id);
        if (member == null)
            return NotFound();
        return View(member.ToMemberViewModel());
    }

    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [Authorize(Policy = "AdminRole")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, [Bind("MemberId,MemberName," +
                                                           "MemberDisplayName, Guest")]
        MemberViewModel member)
    {
        if (id != member.MemberId)
            return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                await masterDataService.UpdateMemberAsync(member.ToDto());
            }
            catch (Exception)
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
            return NotFound();

        var member = await masterDataService.GetMemberAsync(id);
        if (member == null)
            return NotFound();

        return View(member.ToMemberViewModel());
    }

    [Authorize(Policy = "AdminRole")]
    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string id)
    {
        var member = await masterDataService.GetMemberAsync(id);
        if (member != null)
            masterDataService.DeleteMemberAsync(id);
        return RedirectToAction(nameof(Index));
    }
}