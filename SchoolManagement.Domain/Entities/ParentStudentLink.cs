using SchoolManagement.Domain.Common;

namespace SchoolManagement.Domain.Entities;

/// <summary>
/// Many-to-many link between a Parent's Identity user and the student(s) they can access.
/// A student can have more than one linked parent (mother, father, guardian, ...).
/// </summary>
public class ParentStudentLink : BaseEntity
{
    public Guid ParentUserId { get; set; }
    public Guid StudentId { get; set; }
}
