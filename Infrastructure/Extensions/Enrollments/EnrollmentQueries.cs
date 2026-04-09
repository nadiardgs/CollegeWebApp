using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Extensions.Enrollments;

public static class EnrollmentQueries
{
    extension(IQueryable<Enrollment> enrollments)
    {
        public async Task<bool> IsEnrolledAsync(int studentId, int courseId, CancellationToken ct)
        {
            return await enrollments.AnyAsync(e => 
                    e.StudentId == studentId && 
                    e.CourseId == courseId, 
                ct);
        }
    }
}