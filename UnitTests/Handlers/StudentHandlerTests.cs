using Application.Features.Students.Requests;
using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace UnitTests.Handlers;

public class CreateStudentHandlerTests
{
    private readonly CollegeDbContext _context;
    private readonly CreateStudentRequestHandler _handler;
    private readonly CreateStudentRequest _validStudentRequest;

    public CreateStudentHandlerTests()
    {
        var options = new DbContextOptionsBuilder<CollegeDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        _context = new CollegeDbContext(options);
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
        var studentInDb = await _context.Students.FindAsync(result.Student.Id);
        Assert.NotNull(studentInDb);
        Assert.Equal(_validStudentRequest.Name, studentInDb.Name);
    }
}