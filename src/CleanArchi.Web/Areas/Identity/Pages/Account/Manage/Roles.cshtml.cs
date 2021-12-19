#nullable disable

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CleanArchi.Web.Areas.Identity.Pages.Account.Manage;

[Authorize(Roles = Infrastructure.Identity.IdentityConstants.ADMIN_ROLE)]
public class RolesModel : PageModel
{
    private readonly RoleManager<IdentityRole> _rolesManager;

    public RolesModel(RoleManager<IdentityRole> rolesManager)
    {
        _rolesManager = rolesManager;
    }

    public List<IdentityRole> Roles { get; set; }

    [BindProperty]
    public InputModel Input { get; set; }

    public class InputModel
    {
        [Required]
        [DataType(DataType.Text)]
        public string Role { get; set; }
    }

    public void OnGet()
    {
        Roles = _rolesManager.Roles.ToList();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var isExisting = await _rolesManager.RoleExistsAsync(Input.Role);
        if (isExisting)
        {
            ModelState.AddModelError(string.Empty, "Role already exists.");
            Roles = _rolesManager.Roles.ToList();
            return Page();
        }

        await _rolesManager.CreateAsync(new IdentityRole(Input.Role));

        Roles = _rolesManager.Roles.ToList();

        return RedirectToPage();
    }
}
