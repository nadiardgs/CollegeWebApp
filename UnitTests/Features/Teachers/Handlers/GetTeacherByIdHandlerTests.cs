using Application.Constants;
using Application.Exceptions;
using Application.Features.Teachers.Requests;
using Application.Features.Teachers.Responses;
using Domain.Entities;
using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace UnitTests.Features.Teachers.Handlers;

public class GetTeacherByIdHandlerTests
{
    private readonly CollegeDbContext _context;
    private readonly GetTeacherByIdRequestHandler _handler;
    private readonly TeacherDto _teacherRequest;
    
    public GetTeacherByIdHandlerTests()
    {
        var options = new DbContextOptionsBuilder<CollegeDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        _context = new CollegeDbContext(options);
        _handler = new GetTeacherByIdRequestHandler(_context);
        
        _teacherRequest = new TeacherDto(1, "Mary Smith");
    }
    
    [Fact]
    public async Task Handle_ShouldReturnTeacher_WhenExists()
    {
        // Arrange
        var teacher = new Teacher { Name = _teacherRequest.Name };
        _context.Teachers.Add(teacher);
        await _context.SaveChangesAsync(); 

        var query = new GetTeacherByIdRequest(teacher.Id);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(_teacherRequest.Name, result.Name);
    }
    
    [Fact]
    public async Task Handle_Should_NotGetTeacher_WhenDoesntExist()
    {
        // Arrange
        var request = new GetTeacherByIdRequest(_teacherRequest.Id);

        // Act
        var result = await Assert.ThrowsAsync<EntityNotFoundException>(() =>
            _handler.Handle(request, CancellationToken.None));
        
        // Assert
        Assert.Empty(result.Data);
        Assert.Equal(ReturnMessages.EntityNotFound(nameof(Teacher), request.Id), result.Message); 
    }
    
}