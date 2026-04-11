using Application.Constants;
using Application.Features.Teachers.Requests;
using Application.Features.Teachers.Responses;
using Application.Features.Teachers.Validators;
using Domain.Entities;
using Infrastructure;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace UnitTests.Features.Teachers.Validators;

public class UpdateTeacherRequestValidatorTests : IAsyncDisposable
{
    private readonly UpdateTeacherRequestValidator _updateTeacherRequestValidator;
    private readonly TeacherDto _validTeacher1;
    private readonly TeacherDto _validTeacher2;
    private readonly CollegeDbContext _context;
    private readonly Teacher _teacher;
    private readonly SqliteConnection _connection;

    public UpdateTeacherRequestValidatorTests()
    {
        _validTeacher1 = new TeacherDto(1, "John Doe");
        _validTeacher2 = new TeacherDto(2, "Mary Doe");

        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<CollegeDbContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new CollegeDbContext(options);
        _context.Database.EnsureCreated();
        
        _updateTeacherRequestValidator = new UpdateTeacherRequestValidator(_context);
        
        _teacher = new Teacher { Name = _validTeacher1.Name };
    }
    
    [Theory]
    [InlineData("A")]
    [InlineData("Ab")]
    public async Task Validator_ShouldHaveError_WhenNameIsTooShort(string shortName)
    {
        // Arrange
        _context.Teachers.Add(_teacher);
        await _context.SaveChangesAsync();
        
        var request = new UpdateTeacherRequest
        {
            Id = _teacher.Id,
            Name = shortName
        };

        // Act
        var result = await _updateTeacherRequestValidator.ValidateAsync(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == ReturnMessages.MinLength(nameof(Teacher)));
    }

    [Fact]
    public async Task Validator_ShouldBeInvalid_WhenTeacherDoesntExist()
    {
        // Arrange
        var request = new UpdateTeacherRequest
        {
            Id = _validTeacher1.Id,
            Name = _validTeacher1.Name
        };
        
        // Act
        var result = await _updateTeacherRequestValidator.ValidateAsync(request);
        
        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == ReturnMessages.EntityNotFound(nameof(Teacher), _validTeacher1.Id));
    }
    
    [Fact]
    public async Task Validator_ShouldBeValid_WhenNameIsTheSameAsExistingTeacher()
    {
        // Arrange
        var existingTeacher = new Teacher { Name = _validTeacher1.Name };
        _context.Teachers.Add(existingTeacher);
        await _context.SaveChangesAsync();

        var request = new UpdateTeacherRequest
        {
            Id = existingTeacher.Id,
            Name = _validTeacher1.Name
        };

        // Act
        var result = await _updateTeacherRequestValidator.ValidateAsync(request);

        // Assert
        Assert.True(result.IsValid);
    }
    
    [Fact]
    public async Task Validator_ShouldBeValid_WhenNameIsLongEnough()
    {
        // Arrange
        _context.Teachers.Add(_teacher);
        await _context.SaveChangesAsync();
        
        var request = new UpdateTeacherRequest
        {
            Id = _teacher.Id,
            Name = _validTeacher2.Name
        };

        // Act
        var result = await _updateTeacherRequestValidator.ValidateAsync(request);

        // Assert
        Assert.True(result.IsValid);
    }
    
    [Fact]
    public async Task Validator_ShouldBeInvalid_WhenNameIsNotUnique()
    {
        var existingTeacher = new Teacher { Name = _validTeacher1.Name };
        _context.Teachers.Add(existingTeacher);
        await _context.SaveChangesAsync();

        var request = new UpdateTeacherRequest
        {
            Id = _validTeacher2.Id,
            Name = _validTeacher1.Name
        };

        // Act
        var result = await _updateTeacherRequestValidator.ValidateAsync(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == ReturnMessages.UniqueName(nameof(Teacher), existingTeacher.Name));
    }
    
    public async ValueTask DisposeAsync()
    {
        await _context.DisposeAsync();
        await _connection.DisposeAsync();
    }
}