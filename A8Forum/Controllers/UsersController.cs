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
public class UsersController : Controller
{
    private readonly UserManager<A8ForumazurewebsitesnetUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IMasterDataService _masterDataService;

    public UsersController(UserManager<A8ForumazurewebsitesnetUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IMasterDataService masterDataService)
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _masterDataService = masterDataService;
    }

    public async Task<IActionResult> Index()
    {
        var users = await _userManager.Users.ToListAsync();
        var userRolesViewModel = new List<UsersViewModel>();
        foreach (A8ForumazurewebsitesnetUser user in users)
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
    public async Task<IActionResult> Create([Bind("Member, UserName, Password, ConfirmPassword")] CreateUserViewModel input)
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

            var result = await _userManager.CreateAsync(user, input.Password);

            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
        return View(input);
    }

    public async Task<IActionResult> DeleteUser(string? userId)
    {
        if (userId == null)
        {
            return NotFound();
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound();
        }

        return View(new DeleteUserViewModel { UserName = user.UserName, UserId = userId });
    }

    [HttpPost, ActionName("DeleteUser")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user != null)
        {
            await _userManager.DeleteAsync(user);
        }
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> ToggleLockout(string? userId)
    {
        if (userId == null)
        {
            return NotFound();
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound();
        }

        return View(new ToggleLockoutViewModel { UserName = user.UserName, UserId = userId, IsLockedOut = user.LockoutEnd.HasValue });
    }

    [HttpPost, ActionName("ToggleLockout")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleLockoutConfirmed(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user != null)
        {
            await _userManager.SetLockoutEndDateAsync(user, user.LockoutEnd.HasValue ? null : DateTimeOffset.MaxValue);
        }
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> ChangePassword(string? userId)
    {
        if (userId == null)
        {
            return NotFound();
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound();
        }

        return View(new ChangePasswordViewModel { UserName = user.UserName, UserId = userId });
    }

    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(string userId,
        [Bind("UserId, Password, ConfirmPassword, UserName")] ChangePasswordViewModel model)
    {
        if (userId != model.UserId)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return NotFound($"Unable to load user with ID '{userId}'.");
                }

                var result = await _userManager.RemovePasswordAsync(user);
                if (result.Succeeded)
                {
                    result = await _userManager.AddPasswordAsync(user, model.Password);
                }

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(model);
                }
            }
            catch (Exception ex)
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
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            ViewBag.ErrorMessage = $"User with Id = {userId} cannot be found";
            return View("NotFound");
        }
        ViewBag.UserName = user.UserName;
        var model = new List<ManageUserRolesViewModel>();
        foreach (var role in _roleManager.Roles)
        {
            var userRolesViewModel = new ManageUserRolesViewModel
            {
                RoleId = role.Id,
                RoleName = role.Name
            };
            if (await _userManager.IsInRoleAsync(user, role.Name))
            {
                userRolesViewModel.Selected = true;
            }
            else
            {
                userRolesViewModel.Selected = false;
            }
            model.Add(userRolesViewModel);
        }
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> ManageRoles(List<ManageUserRolesViewModel> model, string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return View();
        }
        var roles = await _userManager.GetRolesAsync(user);
        var result = await _userManager.RemoveFromRolesAsync(user, roles);
        if (!result.Succeeded)
        {
            ModelState.AddModelError("", "Cannot remove user existing roles");
            return View(model);
        }
        result = await _userManager.AddToRolesAsync(user, model.Where(x => x.Selected).Select(y => y.RoleName));
        if (!result.Succeeded)
        {
            ModelState.AddModelError("", "Cannot add selected roles to user");
            return View(model);
        }
        return RedirectToAction("Index");
    }

    private async Task<List<string>> GetUserRoles(A8ForumazurewebsitesnetUser user)
    {
        return new List<string>(await _userManager.GetRolesAsync(user));
    }

    private async Task PopulateMembersDropDownListAsync(string? memberId = null)
    {
        var q = (await _masterDataService.GetMembersAsync()).Select(x => x.ToMemberViewModel());
        ViewBag.MemberId = q.ToSelectList(memberId);
    }
}