using SchoolManagement.Domain.Enums;

namespace SchoolManagement.Application.Students.Dtos;

public class StudentQuery
{
    /// <summary>Matches name, admission number, email or guardian name.</summary>
    public string? Search { get; set; }
    public StudentStatus? Status { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
