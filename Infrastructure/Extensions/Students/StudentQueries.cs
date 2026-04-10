using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Extensions.Students;

public static class StudentQueries
{
    extension(IQueryable<Student> students)
    {
        public async Task<bool> IsNameUniqueAsync(string name, int? excludeId, CancellationToken ct)
        {
            return !await students.AnyAsync(s => 
                s.Name == name && s.Id != excludeId, ct);
        }

        public Task<bool> StudentIdExistsAsync(int id, CancellationToken ct)
        {
            return students.AnyAsync(s => s.Id == id, ct);
        }
    }
}