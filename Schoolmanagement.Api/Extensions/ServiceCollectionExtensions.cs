using System.Text.Json.Serialization;
using SchoolManagement.Api.ExceptionHandling;
using SchoolManagement.Application.Students;

namespace SchoolManagement.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.AddControllers()
            .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

        services.AddSwaggerDocumentation();

        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

        // Application services
        services.AddScoped<IStudentService, StudentService>();

        return services;
    }
}
