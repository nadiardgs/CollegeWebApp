using Application.Constants;
using Application.Exceptions;
using Application.Features.Students.Requests;
using Domain.Entities;
using Infrastructure;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace UnitTests.Features.Students.Handlers;

public class CreateStudentHandlerTests : IAsyncDisposable
{
    private readonly CollegeDbContext _context;
    private readonly CreateStudentRequestHandler _handler;
    private readonly CreateStudentRequest _validStudentRequest;
    private readonly  SqliteConnection _connection;

    public CreateStudentHandlerTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<CollegeDbContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new CollegeDbContext(options);
        _context.Database.EnsureCreated();
    
        _handler = new CreateStudentRequestHandler(_context);
        
        _validStudentRequest = new CreateStudentRequest("Alice Smith");
    }

    [Fact]
    public async Task Handle_Should_AddStudentToDatabase()
    {
        // Arrange
        var request = new CreateStudentRequest(_validStudentRequest.Name);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        var studentInDb = await _context.Students
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == result.Id);
        
        Assert.NotNull(studentInDb);
        Assert.Equal(_validStudentRequest.Name, studentInDb.Name); 
        Assert.Equal(studentInDb.Id, result.Id);
        Assert.Equal(studentInDb.Name, result.Name);
        Assert.Equal(1, await _context.Students.CountAsync());
    }
    
    [Fact]
    public async Task Handle_ShouldThrowConflictException_WhenNameExists()
    {
        // Arrange
        await _context.Students.AddAsync(new Student { Name = _validStudentRequest.Name});
        await _context.SaveChangesAsync();
        var request = new CreateStudentRequest(_validStudentRequest.Name);
        
        // Act
        var result = await Assert.ThrowsAsync<UniqueNameException>(() => 
            _handler.Handle(request, CancellationToken.None));
        
        // Assert
        Assert.Equal(result.Message, ReturnMessages.UniqueName(nameof(Student), request.Name));
        Assert.Equal(1, await _context.Students.CountAsync());
    }
    
    [Fact]
    public async Task Handle_Should_ThrowOperationCanceledException_WhenCancelled()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() => 
            _handler.Handle(_validStudentRequest, cts.Token));
    }
    
    public async ValueTask DisposeAsync()
    {
        await _context.DisposeAsync();
        await _connection.DisposeAsync();
    }
}