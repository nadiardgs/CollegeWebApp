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
            Name = "Prof. Alan Turing" 
        };

        var course = new Course
        {
            Title = "Introduction to Computer Science",
            Teacher = teacher
        };

        var student1 = new Student
        {
            Name = "John Smith"
        };
        
        var student2 = new Student
        {
            Name = "Mary Smith"
        };
        
        var grade1 = new Grade 
        {
            Value = 9.5m
        };
        
        var grade2 = new Grade 
        {
            Value = 9.5m
        };

        var enrollment1 = new Enrollment
        {
            StudentId = student1.Id,
            CourseId = course.Id
        };

        var enrollment2 = new Enrollment
        {
            StudentId = student2.Id,
            CourseId = course.Id
        };
        
        await context.Teachers.AddAsync(teacher);
        await context.Courses.AddAsync(course);
        await context.Students.AddRangeAsync(student1, student2);
        await context.Grades.AddRangeAsync(grade1, grade2);
        await context.Enrollments.AddRangeAsync(enrollment1, enrollment2);
        
        await context.SaveChangesAsync();
    }
}