using SchoolManagement.Api.Extensions;
using SchoolManagement.Infrastructure;
using SchoolManagement.Infrastructure.Persistence.Seeding;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiServices();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Must be first so it catches exceptions from everything below.
app.UseExceptionHandler();

// Swagger — enable in all environments so Render can serve /swagger
app.UseSwagger();
app.UseSwaggerUI();

// app.UseHttpsRedirection();   // Render terminates TLS upstream — not needed

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Simple root endpoint for sanity checks
app.MapGet("/", () => Results.Ok(new
{
    service = "SchoolManagement.Api",
    status = "running",
    swagger = "/swagger",
    time = DateTime.UtcNow
}));

// Migrate + seed roles/admin (enabled via Database:InitialiseOnStartup).
if (app.Configuration.GetValue<bool>("Database:InitialiseOnStartup"))
{
    await app.InitialiseDatabaseAsync();
}

app.Run();