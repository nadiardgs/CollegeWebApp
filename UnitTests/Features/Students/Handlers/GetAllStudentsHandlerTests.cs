using Application.Constants;
using Application.Features.Students.Requests;
using Application.Features.Students.Responses;
using Domain.Entities;
using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace UnitTests.Features.Students.Handlers;

public class GetAllStudentsHandlerTests
{
    private readonly CollegeDbContext _context;
    private readonly GetAllStudentsRequestHandler _handler;
    private readonly StudentDto _studentRequest1;
    private readonly StudentDto _studentRequest2;
    
    public GetAllStudentsHandlerTests()
    {
        var options = new DbContextOptionsBuilder<CollegeDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        _context = new CollegeDbContext(options);
        _handler = new GetAllStudentsRequestHandler(_context);
        
        _studentRequest1 = new StudentDto(1, "John Smith");
        _studentRequest2 = new StudentDto(2, "Mary Smith");
    }
    
    [Fact]
    public async Task Handle_ShouldReturnStudents_WhenStudentsExist()
    {
        // Arrange
        var student1 = new Student { Name = _studentRequest1.Name };
        _context.Students.Add(student1);
        await _context.SaveChangesAsync(); 
        
        var student2 = new Student { Name = _studentRequest2.Name };
        _context.Students.Add(student2);
        await _context.SaveChangesAsync(); 

        var query = new GetAllStudentsRequest();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        
        var studentDtos = result.Data.ToList();
        
        Assert.Equal(ReturnMessages.Success(studentDtos.Count, nameof(Student)), result.Message);
        
        var resultStudent1 = studentDtos.FirstOrDefault(s => s.Id == _studentRequest1.Id);
        Assert.NotNull(resultStudent1);
        Assert.Equal(_studentRequest1.Name, resultStudent1.Name);
        
        var resultStudent2 = studentDtos.FirstOrDefault(s => s.Id == _studentRequest2.Id);
        Assert.NotNull(resultStudent2);
        Assert.Equal(_studentRequest2.Name, resultStudent2.Name);
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