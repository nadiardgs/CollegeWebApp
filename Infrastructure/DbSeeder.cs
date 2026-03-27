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
            Name = "John Smith",
            Courses = [course]
        };
        
        var student2 = new Student
        {
            Name = "Mary Smith",
            Courses = [course]
        };
        
        var grade1 = new Grade 
        {
            Value = 9.5m, 
            Student = student1,
            Course = course
        };
        
        var grade2 = new Grade 
        {
            Value = 9.5m, 
            Student = student2, 
            Course = course
        };

        await context.Teachers.AddAsync(teacher);
        await context.Courses.AddAsync(course);
        await context.Students.AddRangeAsync(student1, student2);
        await context.Grades.AddRangeAsync(grade1, grade2);
        
        await context.SaveChangesAsync();
    }
}