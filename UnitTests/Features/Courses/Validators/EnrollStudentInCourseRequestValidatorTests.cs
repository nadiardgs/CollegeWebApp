using Application.Features.Courses.Requests;
using Application.Features.Courses.Responses;
using Application.Features.Courses.Validators;
using Application.Features.Students.Responses;
using Application.Features.Teachers.Responses;
using Domain.Entities;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Time.Testing;
using Xunit.Abstractions;

namespace UnitTests.Features.Courses.Validators;

public class EnrollStudentInCourseRequestValidatorTests : IAsyncDisposable
{
    private readonly EnrollStudentInCourseRequestValidator _enrollStudentInCourseRequestValidator;
    private readonly StudentDto _validStudent;
    private readonly CourseDto _validCourse;
    private readonly TeacherDto _validTeacher;
    private readonly CollegeDbContext _context;
    private readonly FakeTimeProvider _timeProvider;
    
    //TODO scenarios to test:
    /*
     * inexistent course
     * inexistent student
     * no teacher assigned to course
     * student already enrolled
     */
    
    public EnrollStudentInCourseRequestValidatorTests()
    {
        _validStudent = new StudentDto(1, "John Doe Student");
        _validCourse = new CourseDto(1, "Test Course");
        _validTeacher = new TeacherDto(1, "John Doe Teacher");
        
        var options = new DbContextOptionsBuilder<CollegeDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _timeProvider = new FakeTimeProvider();
        
        _context = new CollegeDbContext(options);
        
        _enrollStudentInCourseRequestValidator = new EnrollStudentInCourseRequestValidator(_context);
    }

    [Fact]
    public async Task Validator_ShouldBeValid_WhenTeacherAndStudentExist()
    {
        // Arrange
        _context.Students.Add(new Student
        {
            Id = _validStudent.Id,
            Name = _validStudent.Name
        });
        
        _context.Courses.Add(new Course
        {
            Id = _validCourse.Id,
            Title = _validCourse.Title,
            Teacher = new Teacher
            {
                Id = _validTeacher.Id,
                Name = _validTeacher.Name
            }
        });
        
        await _context.SaveChangesAsync();
        
        var enrollment = new Enrollment
        {
            StudentId = _validStudent.Id,
            CourseId = _validCourse.Id,
            EnrolledAt = _timeProvider.GetUtcNow().DateTime
        };
        
        var enrollmentRequest = new EnrollStudentInCourseRequest(enrollment.CourseId, enrollment.StudentId);
        
        // Act
        var result = await _enrollStudentInCourseRequestValidator.ValidateAsync(enrollmentRequest);
        
        // Assert
        Assert.True(result.IsValid);
    }
    
    public async ValueTask DisposeAsync()
    {
        await _context.DisposeAsync();
    }
}