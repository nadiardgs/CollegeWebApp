using Application.Requests.Students;
using Application.Responses.Students;
using Application.Responses.Students.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using WebApplication3.Controllers;

namespace Tests.Controllers;

public class StudentsControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly StudentsController _controller;

    public StudentsControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new StudentsController(_mediatorMock.Object);
    }

    [Fact]
    public async Task Create_ShouldReturnOk_WhenStudentIsCreated()
    {
        // Arrange
        var command = new CreateStudentRequest("John Doe");
        var expectedResponse = new CreateStudentResponse(
            new StudentDto(
                1, "John Doe")
        );

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<CreateStudentRequest>(), CancellationToken.None))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.Create(command);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(expectedResponse, okResult.Value);
    }

    [Fact]
    public async Task GetAll_ShouldReturnList_WhenStudentsExist()
    {
        // Arrange
        var expectedStudents = new List<StudentDto>
        {
            new(1, "John Doe"),
            new(2, "Jane Doe")
        };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetAllStudentsRequest>(), CancellationToken.None))
            .ReturnsAsync(expectedStudents);

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var actualStudents = Assert.IsAssignableFrom<IEnumerable<StudentDto>>(okResult.Value);
        Assert.Equal(2, actualStudents.Count());
    }
}