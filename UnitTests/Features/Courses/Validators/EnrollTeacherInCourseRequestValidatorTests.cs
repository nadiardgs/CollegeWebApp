using Application.Constants;
using Application.Features.Courses.Requests;
using Application.Features.Courses.Validators;
using Domain.Entities;
using UnitTests.TestBases.Teachers;

namespace UnitTests.Features.Courses.Validators;

public class EnrollTeacherInCourseRequestValidatorTests : TeacherTestBase
{
    private readonly EnrollTeacherInCourseRequestValidator _enrollTeacherInCourseRequestValidator;
    
    public EnrollTeacherInCourseRequestValidatorTests()
    {
        _enrollTeacherInCourseRequestValidator = new EnrollTeacherInCourseRequestValidator(Context);
    }

    [Fact]
    public async Task Validator_ShouldBeValid_WhenCourseHasNoTeacher()
    {
        // Arrange
        SeedCourse(
            ValidCourse.Id, 
            ValidCourse.Title);
        
        SeedTeacher(
            ValidTeacher1.Id, 
            ValidTeacher1.Name);
        
        await Context.SaveChangesAsync();
        
        // Act
        var enrollmentRequest = new EnrollTeacherInCourseRequest(ValidCourse.Id, ValidTeacher1.Id);
        
        // Act
        var result = await _enrollTeacherInCourseRequestValidator.ValidateAsync(enrollmentRequest, CancellationToken.None);
        
        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task Validator_ShouldBeInvalid_WhenCourseHasATeacher()
    {
        // Arrange
        SeedCourseWithTeacher(
            ValidCourse.Id,
            ValidCourse.Title,
            ValidTeacher1.Id,
            ValidTeacher1.Name);
        
        await Context.SaveChangesAsync();
        
        // Act
        var enrollmentRequest = new EnrollTeacherInCourseRequest(ValidCourse.Id, ValidTeacher1.Id);

        var result = await _enrollTeacherInCourseRequestValidator.ValidateAsync(enrollmentRequest, CancellationToken.None);
        
        // Assert
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Contains(result.Errors, e => e.ErrorMessage == ReturnMessages.TeacherAlreadyAssigned(ValidCourse.Id));
    }

    [Fact]
    public async Task Validator_ShouldBeInvalid_WhenCourseDoesntExist()
    {
        // Arrange
        SeedTeacher(
            ValidTeacher1.Id,
            ValidTeacher1.Name);
        
        await Context.SaveChangesAsync();
        
        // Act
        var enrollmentRequest = new EnrollTeacherInCourseRequest(ValidCourse.Id, ValidTeacher1.Id);

        var result = await _enrollTeacherInCourseRequestValidator.ValidateAsync(enrollmentRequest, CancellationToken.None);
        
        // Assert
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Contains(result.Errors, e => e.ErrorMessage == ReturnMessages.EntityNotFound(nameof(Course), ValidCourse.Id));
    }
    
    [Fact]
    public async Task Validator_ShouldBeInvalid_WhenTeacherDoesntExist()
    {
        // Arrange
        SeedCourse(
            ValidCourse.Id,
            ValidCourse.Title);
        
        await Context.SaveChangesAsync();
        
        // Act
        var enrollmentRequest = new EnrollTeacherInCourseRequest(ValidCourse.Id, ValidTeacher1.Id);

        var result = await _enrollTeacherInCourseRequestValidator.ValidateAsync(enrollmentRequest, CancellationToken.None);
        
        // Assert
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Contains(result.Errors, e => e.ErrorMessage == ReturnMessages.EntityNotFound(nameof(Teacher), ValidTeacher1.Id));
    }
}