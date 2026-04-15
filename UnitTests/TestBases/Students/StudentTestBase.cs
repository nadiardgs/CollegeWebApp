using Domain.Entities;
using Microsoft.Extensions.Time.Testing;

namespace UnitTests.TestBases.Students;

public class StudentTestBase : SqliteTestBase
{
    protected readonly FakeTimeProvider TimeProvider = new();

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