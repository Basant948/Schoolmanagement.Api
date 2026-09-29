using Microsoft.EntityFrameworkCore;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Infrastructure.Persistence.Repositories;

public class ParentStudentLinkRepository(ApplicationDbContext context)
    : GenericRepository<ParentStudentLink>(context), IParentStudentLinkRepository
{
    public async Task<IReadOnlyList<Guid>> GetStudentIdsForParentAsync(
        Guid parentUserId, CancellationToken cancellationToken = default) =>
        await DbSet.Where(l => l.ParentUserId == parentUserId)
            .Select(l => l.StudentId)
            .ToListAsync(cancellationToken);

    public Task<bool> LinkExistsAsync(
        Guid parentUserId, Guid studentId, CancellationToken cancellationToken = default) =>
        DbSet.AnyAsync(l => l.ParentUserId == parentUserId && l.StudentId == studentId, cancellationToken);
}
