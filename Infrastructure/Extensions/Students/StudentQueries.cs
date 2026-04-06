using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Extensions.Students;

public static class StudentQueries
{
    public static Task<bool> IsNameUniqueAsync(this IQueryable<Student> students, string name, CancellationToken ct)
    {
        return students.AllAsync(x => x.Name != name, ct);
    }
}