using A8Forum.Areas.Identity.Data;
using A8Forum.Extensions;
using A8Forum.Mappers;
using A8Forum.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Services;

namespace A8Forum.Controllers;

[Authorize(Policy = "AdminRole")]
public class UsersController(UserManager<A8ForumazurewebsitesnetUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IMasterDataService masterDataService)
    : Controller
{
    public async Task<IActionResult> Index()
    {
        var users = await userManager.Users.ToListAsync();
        var userRolesViewModel = new List<UsersViewModel>();
        foreach (var user in users)
        {
            var thisViewModel = new UsersViewModel
            {
                UserName = user.UserName,
                UserId = user.Id,
                Locked = user.LockoutEnd.HasValue,
                Roles = await GetUserRoles(user)
            };
            userRolesViewModel.Add(thisViewModel);
        }

        return View(userRolesViewModel);
    }

    public async Task<IActionResult> Create()
    {
        await PopulateMembersDropDownListAsync();
        return View();
    }

    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind("Member, UserName, Password, ConfirmPassword")]
        CreateUserViewModel input)
    {
        if (ModelState.IsValid)
        {
            // Add user
            var user = new A8ForumazurewebsitesnetUser
            {
                UserName = input.UserName,
                Email = $"{input.UserName}@A8Forum.azurewebsites.net",
                EmailConfirmed = true,
                MemberId = input.Member.MemberId
            };

            var result = await userManager.CreateAsync(user, input.Password);

            if (result.Succeeded)
                return RedirectToAction(nameof(Index));

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);
        }

        return View(input);
    }

    public async Task<IActionResult> DeleteUser(string? userId)
    {
        if (userId == null)
            return NotFound();

        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
            return NotFound();

        return View(new DeleteUserViewModel { UserName = user.UserName, UserId = userId });
    }

    [HttpPost]
    [ActionName("DeleteUser")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user != null)
            await userManager.DeleteAsync(user);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(string? userId)
    {
        if (userId == null)
            return NotFound();

        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
            return NotFound();

        await PopulateMembersDropDownListAsync(user.MemberId);

        return View(new EditUserViewModel
        {
            UserName = user.UserName, UserId = userId, LockedOut = user.LockoutEnd.HasValue, MemberId = user.MemberId
        });
    }

    [HttpPost]
    [ActionName("Edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditConfirmed(string userId, EditUserViewModel u)
    {
        if (ModelState.IsValid)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();

            await userManager.SetLockoutEndDateAsync(user,
                u.LockedOut ? DateTimeOffset.MaxValue : null);
            user.MemberId = u.MemberId;
            await userManager.UpdateAsync(user);

            return RedirectToAction(nameof(Index));
        }

        await PopulateMembersDropDownListAsync(u.MemberId);
        return View(u);
    }

    public async Task<IActionResult> ChangePassword(string? userId)
    {
        if (userId == null)
            return NotFound();

        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
            return NotFound();

        return View(new ChangePasswordViewModel
            { UserName = user.UserName, UserId = userId, Password = "", ConfirmPassword = "" });
    }

    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(string userId,
        [Bind("UserId, Password, ConfirmPassword, UserName")]
        ChangePasswordViewModel model)
    {
        if (userId != model.UserId)
            return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                var user = await userManager.FindByIdAsync(userId);
                if (user == null)
                    return NotFound($"Unable to load user with ID '{userId}'.");

                var result = await userManager.RemovePasswordAsync(user);
                if (result.Succeeded)
                    result = await userManager.AddPasswordAsync(user, model.Password);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);
                    return View(model);
                }
            }
            catch (Exception)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }

        return View(model);
    }

    public async Task<IActionResult> ManageRoles(string userId)
    {
        ViewBag.userId = userId;
        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
        {
            ViewBag.ErrorMessage = $"User with Id = {userId} cannot be found";
            return View("NotFound");
        }

        ViewBag.UserName = user.UserName;
        var model = new List<ManageUserRolesViewModel>();
        foreach (var role in roleManager.Roles)
        {
            var userRolesViewModel = new ManageUserRolesViewModel
            {
                RoleId = role.Id,
                RoleName = role.Name
            };
            if (await userManager.IsInRoleAsync(user, role.Name))
                userRolesViewModel.Selected = true;
            else
                userRolesViewModel.Selected = false;
            model.Add(userRolesViewModel);
        }

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> ManageRoles(List<ManageUserRolesViewModel> model, string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
            return View();
        var roles = await userManager.GetRolesAsync(user);
        var result = await userManager.RemoveFromRolesAsync(user, roles);
        if (!result.Succeeded)
        {
            ModelState.AddModelError("", "Cannot remove user existing roles");
            return View(model);
        }

        result = await userManager.AddToRolesAsync(user, model.Where(x => x.Selected).Select(y => y.RoleName));
        if (!result.Succeeded)
        {
            ModelState.AddModelError("", "Cannot add selected roles to user");
            return View(model);
        }

        return RedirectToAction("Index");
    }

    private async Task<List<string>> GetUserRoles(A8ForumazurewebsitesnetUser user)
    {
        return [..await userManager.GetRolesAsync(user)];
    }

    private async Task PopulateMembersDropDownListAsync(string? memberId = null)
    {
        var q = (await masterDataService.GetMembersAsync()).Select(x => x.ToMemberViewModel());
        ViewBag.MemberId = q.ToSelectList(memberId);
    }
}