using Application.Constants;
using Application.Exceptions;
using Application.Features.Teachers.Requests;
using Domain.Entities;
using Infrastructure;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace UnitTests.Features.Teachers.Handlers;

public class CreateTeacherHandlerTests
{
    private readonly CollegeDbContext _context;
    private readonly CreateTeacherRequestHandler _handler;
    private readonly CreateTeacherRequest _validTeacherRequest;

    public CreateTeacherHandlerTests()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<CollegeDbContext>()
            .UseSqlite(connection)
            .Options;

        _context = new CollegeDbContext(options);
        _context.Database.EnsureCreated();
    
        _handler = new CreateTeacherRequestHandler(_context);
        
        _validTeacherRequest = new CreateTeacherRequest("Alice Smith");
    }

    [Fact]
    public async Task Handle_Should_AddTeacherToDatabase()
    {
        // Arrange
        var request = new CreateTeacherRequest(_validTeacherRequest.Name);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        var teacherInDb = await _context.Teachers
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == result.TeacherDto.Id);
        
        Assert.NotNull(teacherInDb);
        Assert.Equal(_validTeacherRequest.Name, teacherInDb.Name); Assert.Equal(teacherInDb.Id, result.TeacherDto.Id);
        Assert.Equal(teacherInDb.Name, result.TeacherDto.Name);
    }
    
    [Fact]
    public async Task Handle_ShouldThrowConflictException_WhenNameExists()
    {
        // Arrange
        await _context.Teachers.AddAsync(new Teacher { Name = _validTeacherRequest.Name});
        await _context.SaveChangesAsync();
        var request = new CreateTeacherRequest(_validTeacherRequest.Name);
        
        // Act
        var result = await Assert.ThrowsAsync<UniqueNameException>(() => 
            _handler.Handle(request, CancellationToken.None));
        
        // Assert
        Assert.Equal(result.Message, ErrorMessages.UniqueName(nameof(Teacher), request.Name));
    }
    
    [Fact]
    public async Task Handle_Should_ThrowOperationCanceledException_WhenCancelled()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() => 
            _handler.Handle(_validTeacherRequest, cts.Token));
    }
}