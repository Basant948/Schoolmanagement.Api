namespace SchoolManagement.Infrastructure.Persistence.Seeding;

public class SeedAdminOptions
{
    public const string SectionName = "SeedAdmin";

    public string? Email { get; set; }
    public string? UserName { get; set; }
    public string? Password { get; set; }
    public string FirstName { get; set; } = "System";
    public string LastName { get; set; } = "Administrator";
}
