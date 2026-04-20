using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Extensions.Students;

public static class StudentQueries
{
    extension(IQueryable<Student> students)
    {
        public async Task<bool> IsNameUniqueAsync(string name, int? excludeId, CancellationToken ct)
        {
            return await students.AsNoTracking().AnyAsync(s => 
                s.Name == name && s.Id != excludeId, ct);
        }

        public async Task<bool> IdExistsAsync(int id, CancellationToken ct)
        {
            return await students.AsNoTracking().AnyAsync(s => s.Id == id, ct);
        }
    }
}