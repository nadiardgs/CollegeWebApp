using Application.Constants;
using Application.Features.Students.Requests;
using Application.Features.Students.Validators;
using Domain.Entities;
using UnitTests.TestBases.Students;

namespace UnitTests.Features.Students.Validators;

public class CreateStudentRequestValidatorTests : StudentTestBase
{
    private readonly CreateStudentRequestValidator _createStudentRequestValidator;

    public CreateStudentRequestValidatorTests()
    {
        _createStudentRequestValidator = new CreateStudentRequestValidator(Context);
    }
    
    [Theory]
    [InlineData("A")]
    [InlineData("Ab")]
    public async Task Validator_ShouldHaveError_WhenNameIsTooShort(string shortName)
    {
        // Arrange
        var request = new CreateStudentRequest(shortName);

        // Act
        var result = await _createStudentRequestValidator.ValidateAsync(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == ReturnMessages.MinLength(nameof(Student)));
    }

    [Fact]
    public async Task Validator_ShouldBeValid_WhenNameIsLongEnough()
    {
        // Arrange
        var request = new CreateStudentRequest(ValidStudent1.Name);

        // Act
        var result = await _createStudentRequestValidator.ValidateAsync(request);

        // Assert
        Assert.True(result.IsValid);
    }
}