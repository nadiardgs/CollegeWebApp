using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Extensions.Students;

public static class StudentQueries
{
    extension(IQueryable<Student> students)
    {
        public Task<bool> IsNameUniqueAsync(string name, CancellationToken ct)
        {
            return students.AllAsync(x => x.Name != name, ct);
        }

        public Task<bool> StudentIdExistsAsync(int id, CancellationToken ct)
        {
            return students.AnyAsync(s => s.Id == id, ct);
        }
    }
}