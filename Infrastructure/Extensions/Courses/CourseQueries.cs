using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Extensions.Courses;

public static class CourseQueries
{
    extension(IQueryable<Course> courses)
    {
        public async Task<bool> HasTeacherAssignedAsync(int courseId, CancellationToken ct)
        {
            return await courses.AnyAsync(
                c => c.Id == courseId && 
                     c.TeacherId != null 
                     && c.TeacherId != 0, ct);
        }

        public async Task<bool> IdExistsAsync(int courseId, CancellationToken ct)
        {
            return await courses.AnyAsync(c => c.Id == courseId, ct);
        }
    }
}