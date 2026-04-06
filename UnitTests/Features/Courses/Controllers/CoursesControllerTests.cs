using Application.Features.Courses.Requests;
using Application.Features.Courses.Responses;
using Application.Features.Teachers.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using WebApplication3.Controllers;

namespace UnitTests.Features.Courses.Controllers;

public class CoursesControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly CoursesController _controller;
    private readonly CreateCourseRequest _createValidCourseRequest;
    
    public CoursesControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new CoursesController(_mediatorMock.Object);
        _createValidCourseRequest = new CreateCourseRequest("Course Test");
    }

    [Fact]
    public async Task Create_ShouldReturnOk_WhenCourseIsCreated()
    {
        // Arrange
        var expectedResponse = new CreateCourseResponse(1, _createValidCourseRequest.Title);
        
        _mediatorMock.Setup(m => m.Send(It.IsAny<CreateCourseRequest>(), CancellationToken.None))
            .ReturnsAsync(expectedResponse);
        
        // Act
        var result = await _controller.Create(_createValidCourseRequest);
        
        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(expectedResponse, okResult.Value);
    }
}