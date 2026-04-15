using Application.Constants;
using Application.Features.Students.Requests;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using UnitTests.TestBases.Students;

namespace UnitTests.Features.Students.Handlers;

public class GetAllStudentsHandlerTests : StudentTestBase
{
    private readonly GetAllStudentsRequestHandler _handler;
    
    public GetAllStudentsHandlerTests()
    {
        _handler = new GetAllStudentsRequestHandler(Context);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnStudents_WhenStudentsExist()
    {
        // Arrange
        var student1 = new Student
        {
            Name = ValidStudent1.Name
        };
        
        Context.Students.Add(student1);
        
        var student2 = new Student
        {
            Name = ValidStudent2.Name
        };
        
        Context.Students.Add(student2);
        
        await Context.SaveChangesAsync(); 

        var query = new GetAllStudentsRequest();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        
        var studentDtos = result.Data.ToList();
        
        Assert.Equal(ReturnMessages.Success(studentDtos.Count, nameof(Student)), result.Message);
        
        var resultStudent1 = studentDtos.FirstOrDefault(s => s.Id == ValidStudent1.Id);
        Assert.NotNull(resultStudent1);
        Assert.Equal(ValidStudent1.Name, resultStudent1.Name);
        
        var resultStudent2 = studentDtos.FirstOrDefault(s => s.Id == ValidStudent2.Id);
        Assert.NotNull(resultStudent2);
        Assert.Equal(ValidStudent2.Name, resultStudent2.Name);
        
        Assert.Equal(studentDtos.Count, await Context.Students.CountAsync());
    }
    
    [Fact]
    public async Task Handle_ShouldReturnEmpty_WhenNoStudentsExist()
    {
        // Arrange
        var query = new GetAllStudentsRequest();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);
        
        // Assert
        Assert.Equal(result.Message, ReturnMessages.CollectionNotFound(nameof(Student)));
        Assert.Empty(result.Data);
    }
}