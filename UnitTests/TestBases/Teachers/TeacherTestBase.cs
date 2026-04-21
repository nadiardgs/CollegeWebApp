using Application.Features.Courses.Responses;
using Application.Features.Teachers.Responses;
using Domain.Entities;

namespace UnitTests.TestBases.Teachers;

public class TeacherTestBase : SqliteTestBase
{
    protected readonly CourseDto ValidCourse = new(1, "Test Course");
    protected readonly TeacherDto ValidTeacher = new(1, "Test Teacher");
    
    protected void SeedCourseWithoutTeacher(int id, string title)
    {
        Context.Courses.Add(new Course() { Id = id, Title = title });
    }

    protected void SeedCourseWithTeacher(int courseId, string courseTitle, int teacherId, string teacherName)
    {
        Context.Courses.Add(new Course()
        {
            Id = courseId,
            Title = courseTitle,
            Teacher = new Teacher
            {
                Id = teacherId,
                Name = teacherName
            }
        });
    }

    protected void SeedCourse(int courseId, string courseTitle)
    {
        Context.Courses.Add(new Course()
        {
            Id = courseId,
            Title = courseTitle
        });
    }

    protected void SeedTeacher(int teacherId, string teacherName)
    {
        Context.Teachers.Add(new Teacher()
        {
            Id = teacherId,
            Name = teacherName
        });
    }
}