using Application.Constants;
using Application.Entities.Students.Requests;
using Application.Features.Students.Responses;
using Application.Validators;
using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Tests.Validators;

public class StudentValidatorTests
{
    private readonly StudentValidator _studentValidator;
    private readonly StudentDto _validStudent;

    public StudentValidatorTests()
    {
        _validStudent = new StudentDto(1, "John Doe");

        var options = new DbContextOptionsBuilder<CollegeDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new CollegeDbContext(options);
        
        _studentValidator = new StudentValidator(context);
    }
    
    [Theory]
    [InlineData("A")]
    [InlineData("Ab")]
    public async Task Validator_ShouldHaveError_WhenNameIsTooShort(string shortName)
    {
        // Arrange
        var request = new CreateStudentRequest(shortName);

        // Act
        var result = await _studentValidator.ValidateAsync(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == ValidationMessages.StudentNameMinLength);
    }

    [Fact]
    public async Task Validator_ShouldBeValid_WhenNameIsLongEnough()
    {
        // Arrange
        var request = new CreateStudentRequest(_validStudent.Name);

        // Act
        var result = await _studentValidator.ValidateAsync(request);

        // Assert
        Assert.True(result.IsValid);
    }
}