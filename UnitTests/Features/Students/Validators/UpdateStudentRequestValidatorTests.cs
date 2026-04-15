using Application.Constants;
using Application.Features.Students.Requests;
using Application.Features.Students.Validators;
using Domain.Entities;
using UnitTests.TestBases.Students;

namespace UnitTests.Features.Students.Validators;

public class UpdateStudentRequestValidatorTests : StudentTestBase
{
    private readonly UpdateStudentRequestValidator _updateStudentRequestValidator;
    private readonly Student _student;

    public UpdateStudentRequestValidatorTests()
    {
        _updateStudentRequestValidator = new UpdateStudentRequestValidator(Context);
        
        _student = new Student { Name = ValidStudent1.Name };
    }
    
    [Theory]
    [InlineData("A")]
    [InlineData("Ab")]
    public async Task Validator_ShouldHaveError_WhenNameIsTooShort(string shortName)
    {
        // Arrange
        Context.Students.Add(_student);
        await Context.SaveChangesAsync();
        
        var request = new UpdateStudentRequest
        {
            Id = _student.Id,
            Name = shortName
        };

        // Act
        var result = await _updateStudentRequestValidator.ValidateAsync(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == ReturnMessages.MinLength(nameof(Student)));
    }

    [Fact]
    public async Task Validator_ShouldBeInvalid_WhenStudentDoesntExist()
    {
        // Arrange
        var request = new UpdateStudentRequest
        {
            Id = ValidStudent1.Id,
            Name = ValidStudent1.Name
        };
        
        // Act
        var result = await _updateStudentRequestValidator.ValidateAsync(request);
        
        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == ReturnMessages.EntityNotFound(nameof(Student), ValidStudent1.Id));
    }
    
    [Fact]
    public async Task Validator_ShouldBeValid_WhenNameIsTheSameAsExistingStudent()
    {
        // Arrange
        var existingStudent = new Student { Name = ValidStudent1.Name };
        Context.Students.Add(existingStudent);
        await Context.SaveChangesAsync();

        var request = new UpdateStudentRequest
        {
            Id = existingStudent.Id,
            Name = ValidStudent1.Name
        };

        // Act
        var result = await _updateStudentRequestValidator.ValidateAsync(request);

        // Assert
        Assert.True(result.IsValid);
    }
    
    [Fact]
    public async Task Validator_ShouldBeValid_WhenNameIsLongEnough()
    {
        // Arrange
        Context.Students.Add(_student);
        await Context.SaveChangesAsync();
        
        var request = new UpdateStudentRequest
        {
            Id = _student.Id,
            Name = ValidStudent2.Name
        };

        // Act
        var result = await _updateStudentRequestValidator.ValidateAsync(request);

        // Assert
        Assert.True(result.IsValid);
    }
    
    [Fact]
    public async Task Validator_ShouldBeInvalid_WhenNameIsNotUnique()
    {
        var existingStudent = new Student { Name = ValidStudent1.Name };
        Context.Students.Add(existingStudent);
        await Context.SaveChangesAsync();

        var request = new UpdateStudentRequest
        {
            Id = ValidStudent2.Id,
            Name = ValidStudent1.Name
        };

        // Act
        var result = await _updateStudentRequestValidator.ValidateAsync(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == ReturnMessages.UniqueName(nameof(Student), existingStudent.Name));
    }
}