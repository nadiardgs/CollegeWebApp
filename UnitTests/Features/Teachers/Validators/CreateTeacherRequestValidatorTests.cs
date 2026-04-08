using Application.Constants;
using Application.Features.Teachers.Requests;
using Application.Features.Teachers.Responses;
using Application.Features.Teachers.Validators;
using Domain.Entities;
using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace UnitTests.Features.Teachers.Validators;

public class CreateTeacherRequestValidatorTests
{
    private readonly CreateTeacherRequestValidator _createTeacherRequestValidator;
    private readonly TeacherDto _validTeacher;
    private readonly CollegeDbContext _context;

    public CreateTeacherRequestValidatorTests()
    {
        _validTeacher = new TeacherDto(1, "John Doe");

        var options = new DbContextOptionsBuilder<CollegeDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new CollegeDbContext(options);

        _createTeacherRequestValidator = new CreateTeacherRequestValidator(_context);
    }

    [Theory]
    [InlineData("A")]
    [InlineData("Ab")]
    public async Task Validator_ShouldHaveError_WhenNameIsTooShort(string shortName)
    {
        // Arrange
        var request = new CreateTeacherRequest(shortName);

        // Act
        var result = await _createTeacherRequestValidator.ValidateAsync(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == ReturnMessages.MinLength(nameof(Teacher)));
    }

    [Fact]
    public async Task Validator_ShouldBeValid_WhenNameIsLongEnough()
    {
        // Arrange
        var request = new CreateTeacherRequest(_validTeacher.Name);

        // Act
        var result = await _createTeacherRequestValidator.ValidateAsync(request);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validator_ShouldBeInvalid_WhenNameIsNotUnique()
    {
        var existingTeacher = new Teacher { Name = _validTeacher.Name };
        _context.Teachers.Add(existingTeacher);
        await _context.SaveChangesAsync();

        var request = new CreateTeacherRequest(_validTeacher.Name);

        // Act
        var result = await _createTeacherRequestValidator.ValidateAsync(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors,
            e => e.ErrorMessage == ReturnMessages.UniqueName(nameof(Teacher), existingTeacher.Name));
    }
}