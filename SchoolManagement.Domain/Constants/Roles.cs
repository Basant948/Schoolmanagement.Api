namespace SchoolManagement.Domain.Constants;

public static class Roles
{
    public const string Admin = "Admin";
    public const string Teacher = "Teacher";
    public const string Parent = "Parent";
    public const string Accountant = "Accountant";
    public const string Librarian = "Librarian";

    public static readonly IReadOnlyList<string> All = [Admin, Teacher, Parent, Accountant, Librarian];
}
