using Application.Features.Courses.Requests;
using Application.Features.Courses.Responses;
using Application.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using WebApplication3.Controllers;

namespace UnitTests.Features.Courses.Controllers;

public class CoursesControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly CoursesController _controller;
    private readonly CreateCourseRequest _createValidCourseRequest1;
    private readonly CreateCourseRequest _createValidCourseRequest2;
    
    public CoursesControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new CoursesController(_mediatorMock.Object);
        _createValidCourseRequest1 = new CreateCourseRequest("Course Test 1");
        _createValidCourseRequest2 = new CreateCourseRequest("Course Test 2");
    }

    [Fact]
    public async Task Create_ShouldReturnOk_WhenCourseIsCreated()
    {
        // Arrange
        var expectedResponse = new CreateCourseResponse(1, _createValidCourseRequest2.Title);
        
        _mediatorMock.Setup(m => m.Send(It.IsAny<CreateCourseRequest>(), CancellationToken.None))
            .ReturnsAsync(expectedResponse);
        
        // Act
        var result = await _controller.Create(_createValidCourseRequest2);
        
        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(expectedResponse, okResult.Value);
    }

    [Fact]
    public async Task GetAll_ShouldReturnList_WhenCoursesExist()
    {
        // Arrange
        var expectedStudents = new List<CourseDto>
        {
            new(1, _createValidCourseRequest1.Title),
            new(2, _createValidCourseRequest2.Title)
        };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetAllCoursesRequest>(), CancellationToken.None))
            .ReturnsAsync(new ApiResult<IEnumerable<CourseDto>>(expectedStudents));

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var actualCourses = Assert.IsType<ApiResult<IEnumerable<CourseDto>>>(okResult.Value, exactMatch: false);
        Assert.Equal(expectedStudents.Count, actualCourses.Data.Count());
    }
}