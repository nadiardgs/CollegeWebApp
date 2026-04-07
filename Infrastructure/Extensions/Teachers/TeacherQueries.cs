using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Extensions.Teachers;

public static class TeacherQueries
{
    public static Task<bool> IsNameUniqueAsync(this IQueryable<Teacher> students, string name, CancellationToken ct)
    {
        return students.AllAsync(x => x.Name != name, ct);
    }
}