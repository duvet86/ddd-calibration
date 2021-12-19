using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchi.Infrastructure.Identity;

public static class IdentitySeedData
{
    private const string EMAIL = "admin@admin.com";
    private const string PASSWORD = "yourStrong(!)Pa22word";

    public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var userStore = serviceProvider.GetRequiredService<IUserStore<ApplicationUser>>();
        var emailStore= (IUserEmailStore<ApplicationUser>)userStore;

        if (!roleManager.Roles.Any())
        {
            await roleManager.CreateAsync(new IdentityRole(IdentityConstants.ADMIN_ROLE));
            await roleManager.CreateAsync(new IdentityRole(IdentityConstants.USER_ROLE));
        }

        var existingAdmin = await userManager.FindByEmailAsync(EMAIL);
        if (existingAdmin == null)
        {
            var adminUser = Activator.CreateInstance<ApplicationUser>();

            await userStore.SetUserNameAsync(adminUser, EMAIL, CancellationToken.None);
            await emailStore.SetEmailAsync(adminUser, EMAIL, CancellationToken.None);

            var result = await userManager.CreateAsync(adminUser, PASSWORD);

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, IdentityEnums.Roles.Admin.ToString());
            }
        }
    }
}
