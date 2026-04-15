using Application.Features.Courses.Requests;
using Application.Features.Courses.Responses;
using Application.Features.Students.Responses;
using Application.Features.Teachers.Responses;
using Microsoft.EntityFrameworkCore;
using UnitTests.TestBases.Students;

namespace UnitTests.Features.Courses.Handlers;

public class EnrollStudentInCourseHandlerTests : StudentTestBase
{
    private readonly EnrollStudentInCourseRequestHandler _handler;
    private readonly StudentDto _validStudent;
    private readonly TeacherDto _validTeacher;
    private readonly CourseDto _validCourse;

    public EnrollStudentInCourseHandlerTests()
    {
        _handler = new EnrollStudentInCourseRequestHandler(Context);
        _validStudent = new StudentDto(1, "John Doe Student");
        _validTeacher = new TeacherDto(1, "John Doe Teacher");
        _validCourse = new CourseDto(2, "Course 1");
    }

        [Fact]
        public async Task Handler_ShouldBeValid_WhenTeacherAndStudentExist()
        {
            // Arrange
            SeedStudent(
                _validStudent.Id, 
                _validStudent.Name);
            
            SeedCourseWithTeacher(
                _validCourse.Id, 
                _validCourse.Title, 
                _validTeacher.Id,
                _validTeacher.Name);
            
            await Context.SaveChangesAsync();

            var enrollment = new EnrollStudentInCourseRequest(_validCourse.Id, _validStudent.Id);
            
            // Act
            var result = await _handler.Handle(enrollment, CancellationToken.None);
            
            // Assert
            Assert.Equal(_validStudent.Id, result.StudentId);
            Assert.Equal(_validStudent.Name, result.StudentName);
            Assert.Equal(_validCourse.Id, result.CourseId);
            Assert.Equal(_validCourse.Title, result.CourseTitle);
            Assert.Equal(1, await Context.Enrollments.CountAsync());
        }

    [Fact]
    public async Task Handler_ShouldBeInvalid_WhenCourseDoesntExist()
    {
        // Arrange
        SeedCourseWithTeacher(
            _validCourse.Id,
            _validCourse.Title,
            _validTeacher.Id,
            _validTeacher.Name);

        await Context.SaveChangesAsync();

        var enrollmentRequest = new EnrollStudentInCourseRequest(_validCourse.Id, _validStudent.Id);
        
        // Act
        var result = await _handler.Handle(enrollmentRequest, CancellationToken.None);
        
        // Assert
        
    }
}