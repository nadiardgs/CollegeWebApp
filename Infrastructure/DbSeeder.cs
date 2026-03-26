namespace Infrastructure;

using Domain.Entities;
using Microsoft.EntityFrameworkCore;

public static class DbSeeder
{
    public static async Task SeedAsync(CollegeDbContext context)
    {
        await context.Database.MigrateAsync();

        if (await context.Teachers.AnyAsync()) return;

        var teacher = new Teacher 
        { 
            Id = Guid.NewGuid(), 
            Name = "Prof. Alan Turing" 
        };

        var course = new Course
        {
            Id = Guid.NewGuid(),
            Title = "Introduction to Computer Science",
            Teacher = teacher
        };

        var student1 = new Student
        {
            Id = Guid.NewGuid(),
            Name = "John Smith",
            Courses = [course]
        };
        
        var student2 = new Student
        {
            Id = Guid.NewGuid(),
            Name = "Mary Smith",
            Courses = [course]
        };

        await context.Teachers.AddAsync(teacher);
        await context.Courses.AddAsync(course);
        await context.Students.AddAsync(student1);
        await context.Students.AddAsync(student2);
        
        await context.SaveChangesAsync();
    }
}