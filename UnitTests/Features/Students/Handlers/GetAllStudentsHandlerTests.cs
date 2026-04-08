using Application.Constants;
using Application.Exceptions;
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
    private readonly UpsertStudentResponse _studentRequest1;
    private readonly UpsertStudentResponse _studentRequest2;
    
    public GetAllStudentsHandlerTests()
    {
        var options = new DbContextOptionsBuilder<CollegeDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        _context = new CollegeDbContext(options);
        _handler = new GetAllStudentsRequestHandler(_context);
        
        _studentRequest1 = new UpsertStudentResponse(new StudentDto(1, "John Smith"));
        _studentRequest2 = new UpsertStudentResponse(new StudentDto(2, "Mary Smith"));
    }
    
    [Fact]
    public async Task Handle_ShouldReturnStudents_WhenStudentsExist()
    {
        // Arrange
        var student1 = new Student { Name = _studentRequest1.Student.Name };
        _context.Students.Add(student1);
        await _context.SaveChangesAsync(); 
        
        var student2 = new Student { Name = _studentRequest2.Student.Name };
        _context.Students.Add(student2);
        await _context.SaveChangesAsync(); 

        var query = new GetAllStudentsRequest();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        
        var studentDtos = result.Data.ToList();
        
        Assert.Equal(2, studentDtos.Count);
        
        var resultStudent1 = studentDtos.FirstOrDefault(s => s.Id == _studentRequest1.Student.Id);
        Assert.NotNull(resultStudent1);
        Assert.Equal(_studentRequest1.Student.Name, resultStudent1.Name);
        
        var resultStudent2 = studentDtos.FirstOrDefault(s => s.Id == _studentRequest2.Student.Id);
        Assert.NotNull(resultStudent2);
        Assert.Equal(_studentRequest2.Student.Name, resultStudent2.Name);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnEmpty_WhenNoStudentsExist()
    {
        // Arrange
        var query = new GetAllStudentsRequest();

        var result = await _handler.Handle(query, CancellationToken.None);
        
        // Assert
        //Assert.Equal(result.Message, ReturnMessages.CollectionNotFound(nameof(Student)));
    }
}