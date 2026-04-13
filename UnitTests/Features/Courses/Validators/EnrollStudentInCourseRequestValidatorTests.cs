using Application.Constants;
using Application.Features.Courses.Requests;
using Application.Features.Courses.Responses;
using Application.Features.Courses.Validators;
using Application.Features.Students.Responses;
using Application.Features.Teachers.Responses;
using Domain.Entities;
using Microsoft.Extensions.Time.Testing;
using UnitTests.TestBases;

namespace UnitTests.Features.Courses.Validators;

public class EnrollStudentInCourseRequestValidatorTests : ValidatorTestBase
{
    private readonly EnrollStudentInCourseRequestValidator _enrollStudentInCourseRequestValidator;
    private readonly StudentDto _validStudent;
    private readonly StudentDto _invalidStudent;
    private readonly CourseDto _validCourse;
    private readonly CourseDto _invalidCourse;
    private readonly TeacherDto _validTeacher;
    private readonly FakeTimeProvider _timeProvider;
    
    public EnrollStudentInCourseRequestValidatorTests()
    {
        _validStudent = new StudentDto(1, "John Doe Student");
        _invalidStudent = new StudentDto(93, "John Doe Student");
        _validCourse = new CourseDto(1, "Test Course");
        _invalidCourse = new CourseDto(0, "Test Course");
        _validTeacher = new TeacherDto(1, "John Doe Teacher");
        
        _timeProvider = new FakeTimeProvider();
        
        _enrollStudentInCourseRequestValidator = new EnrollStudentInCourseRequestValidator(Context);
    }

    [Fact]
    public async Task Validator_ShouldBeValid_WhenTeacherAndStudentExist()
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

        var enrollmentRequest = new EnrollStudentInCourseRequest(_validCourse.Id, _validStudent.Id);
        
        // Act
        var result = await _enrollStudentInCourseRequestValidator.ValidateAsync(enrollmentRequest);
        
        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task Validator_ShouldBeInvalid_WhenTeacherNotAssigned()
    {
        // Arrange
        SeedStudent(
            _validStudent.Id, 
            _validStudent.Name);

        await Context.SaveChangesAsync();

        var enrollmentRequest = new EnrollStudentInCourseRequest(_validCourse.Id, _validStudent.Id);
        
        // Act
        var result = await _enrollStudentInCourseRequestValidator.ValidateAsync(enrollmentRequest);
        
        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == ReturnMessages.NoTeacherAssigned(_validCourse.Id));
    }
    
    [Fact]
    public async Task Validator_ShouldBeInvalid_WhenCourseDoesntExist()
    {
        // Arrange
        SeedStudent(
            _validStudent.Id,
            _validStudent.Name);

        SeedCourseWithTeacher(
            _invalidCourse.Id,
            _invalidCourse.Title,
            _validTeacher.Id,
            _validTeacher.Name);

        await Context.SaveChangesAsync();

        var enrollmentRequest = new EnrollStudentInCourseRequest(_invalidCourse.Id, _validStudent.Id);

        // Act
        var result = await _enrollStudentInCourseRequestValidator.ValidateAsync(enrollmentRequest);

        // Assert
        Assert.False(result.IsValid);
        Assert.Equal(2, result.Errors.Count);
        Assert.Contains(result.Errors,
            e => e.ErrorMessage == ReturnMessages.EntityNotFound(nameof(Course), _invalidCourse.Id));
        Assert.Contains(result.Errors, e => e.ErrorMessage == ReturnMessages.NoTeacherAssigned(_invalidCourse.Id)); 
    }
    
    [Fact]
    public async Task Validator_ShouldBeInvalid_WhenStudentDoesntExist()
    {
        // Arrange
        SeedCourseWithTeacher(
            _validCourse.Id,
            _validCourse.Title,
            _validTeacher.Id,
            _validTeacher.Name);

        await Context.SaveChangesAsync();

        var enrollmentRequest = new EnrollStudentInCourseRequest(CourseId: _validCourse.Id, StudentId: _validStudent.Id);

        // Act
        var result = await _enrollStudentInCourseRequestValidator.ValidateAsync(enrollmentRequest);

        // Assert
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Contains(result.Errors,
            e => e.ErrorMessage == ReturnMessages.EntityNotFound(nameof(Student), _validStudent.Id));
    }
    
    [Fact]
    public async Task Validator_ShouldBeInvalid_WhenStudentAlreadyEnrolled()
    {
        // Arrange
        SeedStudent(_validStudent.Id, _validStudent.Name);
        SeedCourseWithTeacher(_validCourse.Id, _validCourse.Title, _validTeacher.Id, _validTeacher.Name);
    
        SeedEnrollment(_validStudent.Id, _validCourse.Id);

        await Context.SaveChangesAsync();

        var enrollmentRequest = new EnrollStudentInCourseRequest(_validCourse.Id, _validStudent.Id);
    
        // Act
        var result = await _enrollStudentInCourseRequestValidator.ValidateAsync(enrollmentRequest);
    
        // Assert
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Contains(result.Errors, e => 
            e.ErrorMessage == ReturnMessages.AlreadyEnrolled(_validStudent.Id, _validCourse.Id));
    }
    
    private void SeedStudent(int id, string name)
    {
        Context.Students.Add(new Student { Id = id, Name = name });
    }

    private void SeedCourseWithTeacher(int courseId, string courseTitle, int teacherId, string teacherName)
    {
        Context.Courses.Add(new Course
        {
            Id = courseId,
            Title = courseTitle,
            Teacher = new Teacher { Id = teacherId, Name = teacherName }
        });
    }
    
    private void SeedEnrollment(int studentId, int courseId)
    {
        Context.Enrollments.Add(new Enrollment
        {
            StudentId = studentId,
            CourseId = courseId,
            EnrolledAt = _timeProvider.GetUtcNow().DateTime
        });
    }

    public new async ValueTask DisposeAsync()
    {
        await Context.DisposeAsync();
    }
}