using System.ComponentModel.DataAnnotations;
using Application.Constants;
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
    private readonly CreateStudentRequest _createValidStudentRequest;
    private readonly CreateStudentRequest _createInvalidStudentRequest;

    public StudentsControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new StudentsController(_mediatorMock.Object);
        _createValidStudentRequest = new CreateStudentRequest("John Doe");
        _createInvalidStudentRequest = new CreateStudentRequest("Jo");
    }

    [Fact]
    public async Task Create_ShouldReturnOk_WhenStudentIsCreated()
    {
        // Arrange
        var expectedResponse = new CreateStudentResponse(
            new StudentDto(
                1, _createValidStudentRequest.Name)
        );

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<CreateStudentRequest>(), CancellationToken.None))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.Create(_createValidStudentRequest);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(expectedResponse, okResult.Value);
    }
    
    [Fact]
    public async Task Create_ShouldReturnError_WhenStudentNamingRuleViolated()
    {
        // Arrange
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<CreateStudentRequest>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ValidationException(ValidationMessages.StudentNameMinLength));
        
        // Act
        var result = await Assert.ThrowsAsync<ValidationException>(() =>
            _controller.Create(_createInvalidStudentRequest));
        
        // Assert
        Assert.Equal(ValidationMessages.StudentNameMinLength, result.Message);   
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
        var actualStudents = Assert.IsType<IEnumerable<StudentDto>>(okResult.Value, exactMatch: false);
        Assert.Equal(2, actualStudents.Count());
    }
}