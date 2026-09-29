using SchoolManagement.Api.Extensions;
using SchoolManagement.Infrastructure;
using SchoolManagement.Infrastructure.Persistence.Seeding;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiServices();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Must be first so it catches exceptions from everything below.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Migrate + seed roles/admin (enabled via Database:InitialiseOnStartup).
if (app.Configuration.GetValue<bool>("Database:InitialiseOnStartup"))
{
    await app.InitialiseDatabaseAsync();
}

app.Run();
