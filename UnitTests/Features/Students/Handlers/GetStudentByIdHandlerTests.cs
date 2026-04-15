using Application.Constants;
using Application.Exceptions;
using Application.Features.Students.Requests;
using Domain.Entities;
using UnitTests.TestBases.Students;

namespace UnitTests.Features.Students.Handlers;

public class GetStudentByIdHandlerTests : StudentTestBase
{
    private readonly GetStudentByIdRequestHandler _handler;
    
    public GetStudentByIdHandlerTests()
    {
        _handler = new GetStudentByIdRequestHandler(Context);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnStudent_WhenExists()
    {
        // Arrange
        var student = new Student { Name = ValidStudent1.Name };
        
        Context.Students.Add(student);
        
        await Context.SaveChangesAsync(); 

        var query = new GetStudentByIdRequest(student.Id);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(ValidStudent1.Name, result.Name);
    }
    
    [Fact]
    public async Task Handle_Should_NotGetStudent_WhenDoesntExist()
    {
        // Arrange
        var request = new GetStudentByIdRequest(ValidStudent1.Id);

        // Act
        var result = await Assert.ThrowsAsync<EntityNotFoundException>(() =>
            _handler.Handle(request, CancellationToken.None));
        
        // Assert
        Assert.Empty(result.Data);
        Assert.Equal(ReturnMessages.EntityNotFound(nameof(Student), request.Id), result.Message); 
    }
}