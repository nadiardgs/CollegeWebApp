using Application.Constants;
using Application.Exceptions;
using Application.Features.Students.Requests;
using Application.Features.Students.Responses;
using Domain.Entities;
using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace UnitTests.Handlers;

public class GetStudentByIdHandlerTests
{
    private readonly CollegeDbContext _context;
    private readonly GetStudentByIdRequestHandler _handler;
    private readonly UpsertStudentResponse _studentRequest;
    
    public GetStudentByIdHandlerTests()
    {
        var options = new DbContextOptionsBuilder<CollegeDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        _context = new CollegeDbContext(options);
        _handler = new GetStudentByIdRequestHandler(_context);
        
        _studentRequest = new UpsertStudentResponse(new StudentDto(1, "Mary Smith"));
    }
    
    [Fact]
    public async Task Handle_ShouldReturnStudent_WhenExists()
    {
        // Arrange
        var student = new Student { Name = _studentRequest.Student.Name };
        _context.Students.Add(student);
        await _context.SaveChangesAsync(); 

        var query = new GetStudentByIdRequest(student.Id);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(_studentRequest.Student.Name, result.Name);
    }
    
    [Fact]
    public async Task Handle_Should_NotGetStudent_WhenDoesntExist()
    {
        // Arrange
        var request = new GetStudentByIdRequest(_studentRequest.Student.Id);

        // Act
        var result = await Assert.ThrowsAsync<EntityNotFoundException>(() =>
            _handler.Handle(request, CancellationToken.None));
        
        // Assert
        Assert.Empty(result.Data);
        Assert.Equal(ErrorMessages.EntityNotFound(nameof(Student), request.Id), result.Message); 
    }
    
}