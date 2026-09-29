using Microsoft.EntityFrameworkCore;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Application.Common.Models;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Enums;

namespace SchoolManagement.Infrastructure.Persistence.Repositories;

public class StudentRepository(ApplicationDbContext context)
    : GenericRepository<Student>(context), IStudentRepository
{
    public Task<Student?> GetByAdmissionNumberAsync(
        string admissionNumber, CancellationToken cancellationToken = default) =>
        DbSet.FirstOrDefaultAsync(s => s.AdmissionNumber == admissionNumber, cancellationToken);

    public Task<bool> AdmissionNumberExistsAsync(
        string admissionNumber, Guid? excludeStudentId = null, CancellationToken cancellationToken = default) =>
        DbSet.AnyAsync(
            s => s.AdmissionNumber == admissionNumber && (excludeStudentId == null || s.Id != excludeStudentId),
            cancellationToken);

    public async Task<PagedResult<Student>> SearchAsync(
        string? search, StudentStatus? status, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = DbSet.AsNoTracking();

        if (status is not null)
            query = query.Where(s => s.Status == status);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var pattern = $"%{EscapeLike(search.Trim())}%";
            query = query.Where(s =>
                EF.Functions.ILike(s.FirstName, pattern) ||
                EF.Functions.ILike(s.LastName, pattern) ||
                EF.Functions.ILike(s.AdmissionNumber, pattern) ||
                EF.Functions.ILike(s.GuardianName, pattern) ||
                (s.Email != null && EF.Functions.ILike(s.Email, pattern)));
        }

        var total = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(s => s.LastName).ThenBy(s => s.FirstName).ThenBy(s => s.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Student>(items, page, pageSize, total);
    }

    public async Task<IReadOnlyList<Student>> GetByAdmissionNumbersAsync(
        IEnumerable<string> admissionNumbers, CancellationToken cancellationToken = default)
    {
        var numbers = admissionNumbers.ToList();
        return await DbSet.Where(s => numbers.Contains(s.AdmissionNumber)).ToListAsync(cancellationToken);
    }

    private static string EscapeLike(string value) =>
        value.Replace("\\", "\\\\").Replace("%", "\\%").Replace("_", "\\_");
}
