using Application.Constants;
using Application.Features.Students.Requests;
using Application.Features.Students.Responses;
using Application.Features.Students.Validators;
using Domain.Entities;
using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace UnitTests.Validators;

public class CreateStudentRequestValidatorTests
{
    private readonly CreateStudentRequestValidator _createStudentRequestValidator;
    private readonly StudentDto _validStudent;
    private readonly CollegeDbContext _context;

    public CreateStudentRequestValidatorTests()
    {
        _validStudent = new StudentDto(1, "John Doe");

        var options = new DbContextOptionsBuilder<CollegeDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new CollegeDbContext(options);
        
        _createStudentRequestValidator = new CreateStudentRequestValidator(_context);
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
        Assert.Contains(result.Errors, e => e.ErrorMessage == ValidationMessages.StudentNameMinLength);
    }

    [Fact]
    public async Task Validator_ShouldBeValid_WhenNameIsLongEnough()
    {
        // Arrange
        var request = new CreateStudentRequest(_validStudent.Name);

        // Act
        var result = await _createStudentRequestValidator.ValidateAsync(request);

        // Assert
        Assert.True(result.IsValid);
    }
    
    [Fact]
    public async Task Validator_ShouldBeInvalid_WhenNameIsNotUnique()
    {
        var existingStudent = new Student { Name = _validStudent.Name };
        _context.Students.Add(existingStudent);
        await _context.SaveChangesAsync();

        var request = new CreateStudentRequest(_validStudent.Name);

        // Act
        var result = await _createStudentRequestValidator.ValidateAsync(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == ValidationMessages.StudentAlreadyExists);
    }
}