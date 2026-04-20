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

    protected void SeedCourseWithTeacher(int id, string title)
    {
        Context.Courses.Add(new Course()
        {
            Id = id,
            Title = title,
            Teacher = new Teacher
            {
                Id = ValidTeacher.Id,
                Name =  ValidTeacher.Name
            }
        });
    }
}