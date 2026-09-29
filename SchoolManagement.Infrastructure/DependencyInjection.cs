using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Application.Auth;
using SchoolManagement.Application.Users;
using SchoolManagement.Infrastructure.Authentication;
using SchoolManagement.Infrastructure.Email;
using SchoolManagement.Infrastructure.Identity;
using SchoolManagement.Infrastructure.Users;
using SchoolManagement.Infrastructure.Persistence;
using SchoolManagement.Infrastructure.Persistence.Interceptors;
using SchoolManagement.Infrastructure.Persistence.Repositories;
using SchoolManagement.Infrastructure.Persistence.Seeding;
using SchoolManagement.Infrastructure.Services;

namespace SchoolManagement.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("NeonDb")
            ?? throw new InvalidOperationException("Connection string 'NeonDb' not found. Set it with user-secrets or the ConnectionStrings__NeonDb env var.");

        services.AddHttpContextAccessor();
        services.TryAddSingleton(TimeProvider.System);
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        // Interceptors (order matters: soft delete -> audit columns -> audit trail)
        services.AddScoped<SoftDeleteInterceptor>();
        services.AddScoped<AuditableEntityInterceptor>();
        services.AddScoped<AuditTrailInterceptor>();

        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options.UseNpgsql(connectionString,
                npgsql => npgsql.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));

            options.AddInterceptors(
                sp.GetRequiredService<SoftDeleteInterceptor>(),
                sp.GetRequiredService<AuditableEntityInterceptor>(),
                sp.GetRequiredService<AuditTrailInterceptor>());

            // Identity's join tables (UserRole, UserClaim, ...) have required navigations to the
            // soft-deletable user. That's expected; silence the warning.
            options.ConfigureWarnings(w =>
                w.Ignore(CoreEventId.PossibleIncorrectRequiredNavigationWithQueryFilterInteractionWarning));
        });

        services.AddDataProtection();
        services.AddIdentityCore<ApplicationUser>(options =>
            {
                options.User.RequireUniqueEmail = true;

                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;

                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
            })
            .AddRoles<ApplicationRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        // Repositories + Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // JWT + login
        services.AddOptions<JwtSettings>()
            .BindConfiguration(JwtSettings.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();
        services.AddSingleton<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddJwtAuthentication();

        // User provisioning (invite / manual create) + email
        services.AddScoped<IPasswordGenerator, SecurePasswordGenerator>();
        services.AddScoped<IUserProvisioningService, UserProvisioningService>();
        services.AddResendEmail();

        services.Configure<SeedAdminOptions>(configuration.GetSection(SeedAdminOptions.SectionName));
        services.AddScoped<ApplicationDbContextInitialiser>();

        return services;
    }
}
