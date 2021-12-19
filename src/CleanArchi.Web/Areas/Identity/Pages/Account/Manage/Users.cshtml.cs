#nullable disable

using CleanArchi.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CleanArchi.Web.Areas.Identity.Pages.Account.Manage;

public class UsersModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UsersModel(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public List<UserWithRoles> UsersWithRoles { get; set; } = new();

    public class UserWithRoles
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; }
    }

    public async Task OnGetAsync()
    {
        var users = await _userManager.Users.ToListAsync();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);

            UsersWithRoles.Add(new UserWithRoles()
            {
                UserId = user.Id,
                Email = user.Email,
                Roles = roles.ToList()
            });
        }
    }
}
