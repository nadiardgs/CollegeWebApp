using Application.Constants;
using Application.Features.Teachers.Requests;
using Application.Features.Teachers.Responses;
using Domain.Entities;
using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace UnitTests.Features.Teachers.Handlers;

public class GetAllTeachersHandlerTests : IAsyncDisposable
{
    private readonly CollegeDbContext _context;
    private readonly GetAllTeachersRequestHandler _handler;
    private readonly TeacherDto _teacherRequest1;
    private readonly TeacherDto _teacherRequest2;
    
    public GetAllTeachersHandlerTests()
    {
        var options = new DbContextOptionsBuilder<CollegeDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        _context = new CollegeDbContext(options);
        _handler = new GetAllTeachersRequestHandler(_context);
        
        _teacherRequest1 = new TeacherDto(1, "John Smith");
        _teacherRequest2 = new TeacherDto(2, "Mary Smith");
    }
    
    [Fact]
    public async Task Handle_ShouldReturnTeachers_WhenTeachersExist()
    {
        // Arrange
        var teacher1 = new Teacher { Name = _teacherRequest1.Name };
        _context.Teachers.Add(teacher1);
        await _context.SaveChangesAsync(); 
        
        var teacher2 = new Teacher { Name = _teacherRequest2.Name };
        _context.Teachers.Add(teacher2);
        await _context.SaveChangesAsync(); 

        var query = new GetAllTeachersRequest();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        
        var teacherDtos = result.Data.ToList();
        
        Assert.Equal(ReturnMessages.Success(teacherDtos.Count, nameof(Teacher)), result.Message);
        
        var resultTeacher1 = teacherDtos.FirstOrDefault(s => s.Id == _teacherRequest1.Id);
        Assert.NotNull(resultTeacher1);
        Assert.Equal(_teacherRequest1.Name, resultTeacher1.Name);
        
        var resultTeacher2 = teacherDtos.FirstOrDefault(s => s.Id == _teacherRequest2.Id);
        Assert.NotNull(resultTeacher2);
        Assert.Equal(_teacherRequest2.Name, resultTeacher2.Name);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnEmpty_WhenNoTeachersExist()
    {
        // Arrange
        var query = new GetAllTeachersRequest();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);
        
        // Assert
        Assert.Equal(result.Message, ReturnMessages.CollectionNotFound(nameof(Teacher)));
        Assert.Empty(result.Data);
    }

    public async ValueTask DisposeAsync()
    {
        await _context.DisposeAsync();
    }
}