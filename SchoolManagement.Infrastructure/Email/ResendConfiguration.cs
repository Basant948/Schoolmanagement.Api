using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Resend;
using SchoolManagement.Application.Common.Interfaces;

namespace SchoolManagement.Infrastructure.Email;

public static class ResendConfiguration
{
    public static IServiceCollection AddResendEmail(this IServiceCollection services)
    {
        services.AddOptions<ResendSettings>()
            .BindConfiguration(ResendSettings.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddHttpClient<ResendClient>();
        services.AddOptions<ResendClientOptions>()
            .Configure<IOptions<ResendSettings>>((clientOptions, resendSettings) =>
            {
                clientOptions.ApiToken = resendSettings.Value.ApiKey;
            });
        services.AddTransient<IResend, ResendClient>();

        services.AddScoped<IEmailService, ResendEmailService>();

        return services;
    }
}
