using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SchoolManagement.Domain.Constants;
using SchoolManagement.Infrastructure.Identity;

namespace SchoolManagement.Infrastructure.Persistence.Seeding;

public class ApplicationDbContextInitialiser(
    ApplicationDbContext context,
    UserManager<ApplicationUser> userManager,
    RoleManager<ApplicationRole> roleManager,
    IOptions<SeedAdminOptions> seedOptions,
    ILogger<ApplicationDbContextInitialiser> logger)
{
    private readonly SeedAdminOptions _admin = seedOptions.Value;

    public async Task InitialiseAsync()
    {
        try
        {
            await context.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while migrating the database.");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await SeedRolesAsync();
            await SeedAdminUserAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    private async Task SeedRolesAsync()
    {
        foreach (var roleName in Roles.All)
        {
            if (await roleManager.RoleExistsAsync(roleName)) continue;

            var result = await roleManager.CreateAsync(new ApplicationRole(roleName));
            ThrowIfFailed(result, $"create role '{roleName}'");
            logger.LogInformation("Seeded role {Role}", roleName);
        }
    }

    private async Task SeedAdminUserAsync()
    {
        if (string.IsNullOrWhiteSpace(_admin.Email) || string.IsNullOrWhiteSpace(_admin.Password))
        {
            logger.LogWarning("SeedAdmin:Email / SeedAdmin:Password not configured - skipping default admin.");
            return;
        }

        // IgnoreQueryFilters so a soft-deleted admin is restored instead of colliding on the unique email.
        var normalizedEmail = userManager.NormalizeEmail(_admin.Email);
        var admin = await userManager.Users.IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.NormalizedEmail == normalizedEmail);

        if (admin is null)
        {
            admin = new ApplicationUser
            {
                UserName = string.IsNullOrWhiteSpace(_admin.UserName) ? _admin.Email : _admin.UserName,
                Email = _admin.Email,
                EmailConfirmed = true,
                FirstName = _admin.FirstName,
                LastName = _admin.LastName
            };

            ThrowIfFailed(await userManager.CreateAsync(admin, _admin.Password), "create the default admin");
            logger.LogInformation("Seeded default admin {Email}", _admin.Email);
        }
        else if (admin.IsDeleted)
        {
            admin.IsDeleted = false;
            admin.DeletedAtUtc = null;
            admin.DeletedBy = null;
            ThrowIfFailed(await userManager.UpdateAsync(admin), "restore the default admin");
        }

        if (!await userManager.IsInRoleAsync(admin, Roles.Admin))
            ThrowIfFailed(await userManager.AddToRoleAsync(admin, Roles.Admin), "assign the Admin role");
    }

    private static void ThrowIfFailed(IdentityResult result, string action)
    {
        if (result.Succeeded) return;
        var errors = string.Join("; ", result.Errors.Select(e => e.Description));
        throw new InvalidOperationException($"Failed to {action}: {errors}");
    }
}
