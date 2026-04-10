using Application.Constants;
using Application.Features.Students.Requests;
using Application.Features.Students.Responses;
using Application.Features.Students.Validators;
using Domain.Entities;
using Infrastructure;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace UnitTests.Features.Students.Validators;

public class UpdateStudentRequestValidatorTests : IAsyncDisposable
{
    private readonly UpdateStudentRequestValidator _updateStudentRequestValidator;
    private readonly StudentDto _validStudent1;
    private readonly StudentDto _validStudent2;
    private readonly CollegeDbContext _context;
    private readonly Student _student;
    private readonly SqliteConnection _connection;

    public UpdateStudentRequestValidatorTests()
    {
        _validStudent1 = new StudentDto(1, "John Doe");
        _validStudent2 = new StudentDto(2, "Mary Doe");

        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<CollegeDbContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new CollegeDbContext(options);
        _context.Database.EnsureCreated();
        
        _updateStudentRequestValidator = new UpdateStudentRequestValidator(_context);
        
        _student = new Student { Name = _validStudent1.Name };
    }
    
    [Theory]
    [InlineData("A")]
    [InlineData("Ab")]
    public async Task Validator_ShouldHaveError_WhenNameIsTooShort(string shortName)
    {
        // Arrange
        _context.Students.Add(_student);
        await _context.SaveChangesAsync();
        
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
            Id = _validStudent1.Id,
            Name = _validStudent1.Name
        };
        
        // Act
        var result = await _updateStudentRequestValidator.ValidateAsync(request);
        
        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == ReturnMessages.EntityNotFound(nameof(Student), _validStudent1.Id));
    }
    
    [Fact]
    public async Task Validator_ShouldBeValid_WhenNameIsTheSameAsExistingStudent()
    {
        // Arrange
        var existingStudent = new Student { Name = _validStudent1.Name };
        _context.Students.Add(existingStudent);
        await _context.SaveChangesAsync();

        var request = new UpdateStudentRequest
        {
            Id = existingStudent.Id,
            Name = _validStudent1.Name
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
        _context.Students.Add(_student);
        await _context.SaveChangesAsync();
        
        var request = new UpdateStudentRequest
        {
            Id = _student.Id,
            Name = _validStudent2.Name
        };

        // Act
        var result = await _updateStudentRequestValidator.ValidateAsync(request);

        // Assert
        Assert.True(result.IsValid);
    }
    
    [Fact]
    public async Task Validator_ShouldBeInvalid_WhenNameIsNotUnique()
    {
        var existingStudent = new Student { Name = _validStudent1.Name };
        _context.Students.Add(existingStudent);
        await _context.SaveChangesAsync();

        var request = new UpdateStudentRequest
        {
            Id = _validStudent2.Id,
            Name = _validStudent1.Name
        };

        // Act
        var result = await _updateStudentRequestValidator.ValidateAsync(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == ReturnMessages.UniqueName(nameof(Student), existingStudent.Name));
    }
    
    public async ValueTask DisposeAsync()
    {
        await _context.DisposeAsync();
        await _connection.DisposeAsync();
    }
}