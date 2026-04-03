using System.ComponentModel.DataAnnotations;
using Application.Constants;
using Application.Entities.Students.Requests;
using Application.Features.Students.Requests;
using Application.Features.Students.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using WebApplication3.Controllers;

namespace UnitTests.Controllers;

public class StudentsControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly StudentsController _controller;
    private readonly CreateStudentRequest _createValidStudentRequest1;
    private readonly CreateStudentRequest _createValidStudentRequest2;
    private readonly CreateStudentRequest _createInvalidStudentRequest;

    public StudentsControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new StudentsController(_mediatorMock.Object);
        _createValidStudentRequest1 = new CreateStudentRequest("John Doe");
        _createValidStudentRequest2 = new CreateStudentRequest("Jane Doe");
        _createInvalidStudentRequest = new CreateStudentRequest("Jo");
    }

    [Fact]
    public async Task Create_ShouldReturnOk_WhenStudentIsCreated()
    {
        // Arrange
        var expectedResponse = new UpsertStudentResponse(
            new StudentDto(
                1, _createValidStudentRequest1.Name)
        );

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<CreateStudentRequest>(), CancellationToken.None))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.Create(_createValidStudentRequest1);

        // Assert
        var okResult = Assert.IsType<CreatedAtActionResult>(result);
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
    public async Task Create_ShouldReturnError_WhenStudentNameIsNotUnique()
    {
        // Arrange
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<CreateStudentRequest>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ValidationException(ValidationMessages.StudentAlreadyExists));
        
        // Act
        var result = await Assert.ThrowsAsync<ValidationException>(() =>
            _controller.Create(_createValidStudentRequest1));
        
        // Assert
        Assert.Equal(ValidationMessages.StudentAlreadyExists, result.Message);   
    }

    [Fact]
    public async Task GetAll_ShouldReturnList_WhenStudentsExist()
    {
        // Arrange
        var expectedStudents = new List<StudentDto>
        {
            new(1, _createValidStudentRequest1.Name),
            new(2, _createValidStudentRequest2.Name)
        };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetAllStudentsRequest>(), CancellationToken.None))
            .ReturnsAsync(expectedStudents);

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var actualStudents = Assert.IsType<IEnumerable<StudentDto>>(okResult.Value, exactMatch: false);
        Assert.Equal(expectedStudents.Count, actualStudents.Count());
    }
}