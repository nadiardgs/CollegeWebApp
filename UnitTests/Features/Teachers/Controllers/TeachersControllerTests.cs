using Application.Constants;
using Application.Exceptions;
using Application.Features.Teachers.Requests;
using Application.Features.Teachers.Responses;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using WebApplication3.Controllers;

namespace UnitTests.Features.Teachers.Controllers;

public class TeachersControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly TeachersController _controller;
    private readonly CreateTeacherRequest _createValidTeacherRequest1;
    private readonly CreateTeacherRequest _createValidTeacherRequest2;
    private readonly CreateTeacherRequest _createInvalidTeacherRequest;
    
    public TeachersControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new TeachersController(_mediatorMock.Object);
        _createValidTeacherRequest1 = new CreateTeacherRequest("John Doe");
        _createValidTeacherRequest2 = new CreateTeacherRequest("Jane Doe");
        _createInvalidTeacherRequest = new CreateTeacherRequest("Jo");
    }
    
    [Fact]
    public async Task Create_ShouldReturnOk_WhenTeacherIsCreated()
    {
        // Arrange
        var expectedResponse = new CreateTeacherResponse(
            new TeacherDto(
                1, _createValidTeacherRequest1.Name)
        );

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<CreateTeacherRequest>(), CancellationToken.None))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.Create(_createValidTeacherRequest1);

        // Assert
        var okResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(expectedResponse, okResult.Value);
    }
    
    [Fact]
    public async Task Create_ShouldReturnError_WhenTeacherNamingRuleViolated()
    {
        // Arrange
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<CreateTeacherRequest>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new MinLengthException(nameof(Teacher)));
        
        // Act
        var result = await Assert.ThrowsAsync<MinLengthException>(() =>
            _controller.Create(_createInvalidTeacherRequest));
        
        // Assert
        Assert.Equal(ReturnMessages.MinLength(nameof(Teacher)), result.Message);
    }
    
    [Fact]
    public async Task GetAll_ShouldReturnList_WhenTeachersExist()
    {
        // Arrange
        var expectedTeachers = new List<TeacherDto>
        {
            new(1, _createValidTeacherRequest1.Name),
            new(2, _createValidTeacherRequest2.Name)
        };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetAllTeachersRequest>(), CancellationToken.None))
            .ReturnsAsync(expectedTeachers);

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var actualTeachers = Assert.IsType<IEnumerable<TeacherDto>>(okResult.Value, exactMatch: false);
        Assert.Equal(2, actualTeachers.Count());
    }
}