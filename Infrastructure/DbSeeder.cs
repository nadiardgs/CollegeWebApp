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
        
        var grade1 = new Grade 
        { 
            Id = Guid.NewGuid(), 
            Value = 9.5m, 
            StudentId = student1.Id, 
            CourseId = course.Id
        };
        
        var grade2 = new Grade 
        { 
            Id = Guid.NewGuid(), 
            Value = 9.5m, 
            StudentId = student2.Id, 
            CourseId = course.Id
        };

        await context.Teachers.AddAsync(teacher);
        await context.Courses.AddAsync(course);
        await context.Students.AddAsync(student1);
        await context.Students.AddAsync(student2);
        await context.Grades.AddAsync(grade1);
        await context.Grades.AddAsync(grade2);
        
        await context.SaveChangesAsync();
    }
}