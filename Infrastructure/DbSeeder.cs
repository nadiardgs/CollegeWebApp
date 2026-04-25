namespace Infrastructure;

using Domain.Entities;
using Microsoft.EntityFrameworkCore;

public static class DbSeeder
{
    public static async Task SeedAsync(CollegeDbContext context)
    {
        if (await context.Database.CanConnectAsync())
        {
            var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
            if (pendingMigrations.Any())
            {
                await context.Database.MigrateAsync();
            }
        }

        if (await context.Teachers.AnyAsync()) return;

        var teacher = new Teacher { Name = "Alan Turing" };

        var course = new Course
        {
            Title = "Introduction to Computer Science",
            Teacher = teacher
        };

        var student1 = new Student { Name = "John Smith" };
        var student2 = new Student { Name = "Mary Smith" };
    
        // 1. Link Enrollment to Objects, not IDs
        var enrollment1 = new Enrollment
        {
            Student = student1,
            Course = course
        };

        var enrollment2 = new Enrollment
        {
            Student = student2,
            Course = course
        };

        var grade1 = new Grade { Value = 9.5m, Enrollment = enrollment1 };
        var grade2 = new Grade { Value = 9.5m, Enrollment = enrollment2 };
        
        await context.Teachers.AddAsync(teacher);
        await context.Courses.AddAsync(course);
        await context.Students.AddRangeAsync(student1, student2);
        await context.Enrollments.AddRangeAsync(enrollment1, enrollment2);
        await context.Grades.AddRangeAsync(grade1, grade2);
    
        await context.SaveChangesAsync();
    }                    
}