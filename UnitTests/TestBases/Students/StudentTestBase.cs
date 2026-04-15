using Application.Features.Courses.Responses;
using Application.Features.Students.Responses;
using Application.Features.Teachers.Responses;
using Domain.Entities;
using Microsoft.Extensions.Time.Testing;

namespace UnitTests.TestBases.Students;

public class StudentTestBase : SqliteTestBase
{
    protected readonly FakeTimeProvider TimeProvider = new();
    protected readonly StudentDto ValidStudent = new(1, "John Doe Student");
    protected readonly CourseDto ValidCourse = new(1, "Test Course");
    protected readonly TeacherDto ValidTeacher = new(1, "John Doe Teacher");

    protected void SeedStudent(int id, string name)
    {
        Context.Students.Add(new Student { Id = id, Name = name });
    }

    protected void SeedCourseWithTeacher(int courseId, string courseTitle, int teacherId, string teacherName)
    {
        Context.Courses.Add(new Course
        {
            Id = courseId,
            Title = courseTitle,
            Teacher = new Teacher { Id = teacherId, Name = teacherName }
        });
    }

    protected void SeedTeacher(int teacherId, string teacherName)
    {
        Context.Teachers.Add(new Teacher
        {
            Id = teacherId, 
            Name = teacherName
        });
    }

    protected void SeedCourse(int courseId, string courseTitle)
    {
        Context.Courses.Add(new Course
        {
            Id = courseId,
            Title = courseTitle
        });
    }
    
    protected void SeedEnrollment(int studentId, int courseId)
    {
        Context.Enrollments.Add(new Enrollment
        {
            StudentId = studentId,
            CourseId = courseId,
            EnrolledAt = TimeProvider.GetUtcNow().DateTime
        });
    }
}