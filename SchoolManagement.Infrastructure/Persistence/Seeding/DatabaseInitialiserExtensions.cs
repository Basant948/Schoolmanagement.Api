using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace SchoolManagement.Infrastructure.Persistence.Seeding;

public static class DatabaseInitialiserExtensions
{
    /// <summary>Applies pending migrations, then seeds roles and the default admin.</summary>
    public static async Task InitialiseDatabaseAsync(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();

        await initialiser.InitialiseAsync();
        await initialiser.SeedAsync();
    }
}
