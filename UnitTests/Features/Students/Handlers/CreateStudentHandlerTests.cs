using Application.Constants;
using Application.Exceptions;
using Application.Features.Students.Requests;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using UnitTests.TestBases.Students;

namespace UnitTests.Features.Students.Handlers;

public class CreateStudentHandlerTests : StudentTestBase
{
    private readonly CreateStudentRequestHandler _handler;

    public CreateStudentHandlerTests()
    {
        _handler = new CreateStudentRequestHandler(Context);
    }

    [Fact]
    public async Task Handle_Should_AddStudentToDatabase()
    {
        // Arrange
        var request = new CreateStudentRequest(ValidStudent1.Name);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        var studentInDb = await Context.Students
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == result.Id);
        
        Assert.NotNull(studentInDb);
        Assert.Equal(ValidStudent1.Name, studentInDb.Name); 
        Assert.Equal(studentInDb.Id, result.Id);
        Assert.Equal(studentInDb.Name, result.Name);
        Assert.Equal(1, await Context.Students.CountAsync());
    }
    
    [Fact]
    public async Task Handle_ShouldThrowConflictException_WhenNameExists()
    {
        // Arrange
        await Context.Students.AddAsync(new Student
        {
            Name = ValidStudent1.Name
        });
        
        await Context.SaveChangesAsync();
        
        var request = new CreateStudentRequest(ValidStudent1.Name);
        
        // Act
        var result = await Assert.ThrowsAsync<UniqueNameException>(() => 
            _handler.Handle(request, CancellationToken.None));
        
        // Assert
        Assert.Equal(result.Message, ReturnMessages.UniqueName(nameof(Student), request.Name));
        Assert.Equal(1, await Context.Students.CountAsync());
    }
    
    [Fact]
    public async Task Handle_Should_ThrowOperationCanceledException_WhenCancelled()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() => 
            _handler.Handle(new CreateStudentRequest(ValidStudent1.Name), cts.Token));
    }
}