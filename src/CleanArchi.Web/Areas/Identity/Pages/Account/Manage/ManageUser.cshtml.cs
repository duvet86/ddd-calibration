#nullable disable

using CleanArchi.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CleanArchi.Web.Areas.Identity.Pages.Account.Manage;

public class ManageUserModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public ManageUserModel(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public string Username { get; set; }
    public List<SelectListItem> Options { get; set; }

    [BindProperty]
    public string Role { get; set; }

    [BindProperty]
    public string Action { get; set; }

    public async Task<IActionResult> OnGetAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{userId}'.");
        }

        await LoadAsync(user);

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (Action == "delete")
        {
            await _userManager.DeleteAsync(user);

            return RedirectToPage("./Users");
        }

        var roles = await _userManager.GetRolesAsync(user);

        // Remove existing roles before adding new one.
        await _userManager.RemoveFromRolesAsync(user, roles);

        await _userManager.AddToRoleAsync(user, Role);

        await LoadAsync(user);

        return Page();
    }

    private async Task LoadAsync(ApplicationUser user)
    {
        Username = await _userManager.GetUserNameAsync(user);
        var roles = await _userManager.GetRolesAsync(user);

        var availableRoles = await _roleManager.Roles.ToListAsync();

        Options = availableRoles
            .OrderBy(p => p.Name)
            .Select(p => new SelectListItem()
            {
                Value = p.Name,
                Text = p.Name,
                Selected = p.Name == roles.FirstOrDefault()
            })
            .ToList();
    }
}
