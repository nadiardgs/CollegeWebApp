using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Extensions.Teachers;

public static class TeacherQueries
{
    extension(IQueryable<Teacher> teachers)
    {
        public async Task<bool> IsNameUniqueAsync(string name, int? excludeId, CancellationToken ct)
        {
            return !await teachers.AnyAsync(s => 
                s.Name == name && s.Id != excludeId, ct);
        }

        public Task<bool> TeacherIdExistsAsync(int id, CancellationToken ct)
        {
            return teachers.AnyAsync(s => s.Id == id, ct);
        }
    }
}