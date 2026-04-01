using Application.Requests.Courses;
using Application.Responses.Courses;
using Application.Responses.Teachers.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using WebApplication3.Controllers;

namespace Tests.Controllers;

public class CoursesControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly CoursesController _controller;
    private readonly CreateCourseRequest _createValidCourseRequest;
    private readonly TeacherDto _validTeacherDto;
    
    public CoursesControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new CoursesController(_mediatorMock.Object);
        _createValidCourseRequest = new CreateCourseRequest("Course Test", 1);
        _validTeacherDto = new TeacherDto(1, "Teacher test");
    }

    [Fact]
    public async Task Create_ShouldReturnOk_WhenCourseIsCreated()
    {
        // Arrange
        var expectedResponse = new CreateCourseResponse(1, _createValidCourseRequest.Title, _validTeacherDto.Name);
        
        _mediatorMock.Setup(m => m.Send(It.IsAny<CreateCourseRequest>(), CancellationToken.None))
            .ReturnsAsync(expectedResponse);
        
        // Act
        var result = await _controller.Create(_createValidCourseRequest);
        
        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(expectedResponse, okResult.Value);
    }
}