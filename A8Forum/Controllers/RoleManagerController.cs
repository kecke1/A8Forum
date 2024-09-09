using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace A8Forum.Controllers;

[Authorize(Policy = "AdminRole")]
public class RoleManagerController(RoleManager<IdentityRole> roleManager) : Controller
{
    public async Task<IActionResult> Index()
    {
        var roles = await roleManager.Roles.ToListAsync();
        return View(roles);
    }

    [HttpPost]
    public async Task<IActionResult> AddRole(string roleName)
    {
        if (roleName != null)
            await roleManager.CreateAsync(new IdentityRole(roleName.Trim()));
        return RedirectToAction("Index");
    }
}