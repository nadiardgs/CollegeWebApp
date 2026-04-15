using Application.Constants;
using Application.Features.Courses.Requests;
using Application.Features.Courses.Validators;
using Domain.Entities;
using UnitTests.TestBases.Students;

namespace UnitTests.Features.Courses.Validators;

public class EnrollStudentInCourseRequestValidatorTests : StudentTestBase
{
    private readonly EnrollStudentInCourseRequestValidator _enrollStudentInCourseRequestValidator;
    
    public EnrollStudentInCourseRequestValidatorTests()
    {
        _enrollStudentInCourseRequestValidator = new EnrollStudentInCourseRequestValidator(Context);
    }

    [Fact]
    public async Task Validator_ShouldBeValid_WhenTeacherAndStudentExist()
    {
        // Arrange
        SeedStudent(
            ValidStudent1.Id, 
            ValidStudent1.Name);
        
        SeedCourseWithTeacher(
            ValidCourse.Id,
            ValidCourse.Title,
            ValidTeacher.Id,
            ValidTeacher.Name);

        await Context.SaveChangesAsync();

        var enrollmentRequest = new EnrollStudentInCourseRequest(ValidCourse.Id, ValidStudent1.Id);
        
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
            ValidStudent1.Id, 
            ValidStudent1.Name);
        
        SeedCourse(
            ValidCourse.Id,
            ValidCourse.Title);

        await Context.SaveChangesAsync();

        var enrollmentRequest = new EnrollStudentInCourseRequest(ValidCourse.Id, ValidStudent1.Id);
        
        // Act
        var result = await _enrollStudentInCourseRequestValidator.ValidateAsync(enrollmentRequest);
        
        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == ReturnMessages.NoTeacherAssigned(ValidCourse.Id));
    }
    
    [Fact]
    public async Task Validator_ShouldBeInvalid_WhenCourseDoesntExist()
    {
        // Arrange
        SeedStudent(
            ValidStudent1.Id,
            ValidStudent1.Name);

        SeedTeacher(
            ValidTeacher.Id,
            ValidTeacher.Name);

        await Context.SaveChangesAsync();

        var enrollmentRequest = new EnrollStudentInCourseRequest(ValidCourse.Id, ValidStudent1.Id);

        // Act
        var result = await _enrollStudentInCourseRequestValidator.ValidateAsync(enrollmentRequest);

        // Assert
        Assert.False(result.IsValid);
        Assert.Equal(2, result.Errors.Count);
        Assert.Contains(result.Errors,
            e => e.ErrorMessage == ReturnMessages.EntityNotFound(nameof(Course), ValidCourse.Id));
        Assert.Contains(result.Errors, e => e.ErrorMessage == ReturnMessages.NoTeacherAssigned(ValidCourse.Id)); 
    }
    
    [Fact]
    public async Task Validator_ShouldBeInvalid_WhenStudentDoesntExist()
    {
        // Arrange
        SeedCourseWithTeacher(
            ValidCourse.Id,
            ValidCourse.Title,
            ValidTeacher.Id,
            ValidTeacher.Name);

        await Context.SaveChangesAsync();

        var enrollmentRequest = new EnrollStudentInCourseRequest(ValidCourse.Id, ValidStudent1.Id);

        // Act
        var result = await _enrollStudentInCourseRequestValidator.ValidateAsync(enrollmentRequest);

        // Assert
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Contains(result.Errors,
            e => e.ErrorMessage == ReturnMessages.EntityNotFound(nameof(Student), ValidStudent1.Id));
    }
    
    [Fact]
    public async Task Validator_ShouldBeInvalid_WhenStudentAlreadyEnrolled()
    {
        // Arrange
        SeedStudent(
            ValidStudent1.Id, 
            ValidStudent1.Name);
        
        SeedCourseWithTeacher(
            ValidCourse.Id, 
            ValidCourse.Title, 
            ValidTeacher.Id, 
            ValidTeacher.Name);
    
        SeedEnrollment(
            ValidStudent1.Id, 
            ValidCourse.Id);

        await Context.SaveChangesAsync();

        var enrollmentRequest = new EnrollStudentInCourseRequest(ValidCourse.Id, ValidStudent1.Id);
    
        // Act
        var result = await _enrollStudentInCourseRequestValidator.ValidateAsync(enrollmentRequest);
    
        // Assert
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Contains(result.Errors, e => 
            e.ErrorMessage == ReturnMessages.AlreadyEnrolled(ValidStudent1.Id, ValidCourse.Id));
    }
}