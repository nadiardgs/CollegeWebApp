using Application.Constants;
using Application.Exceptions;
using Application.Features.Students.Requests;
using Application.Features.Students.Responses;
using Application.Models;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using WebApplication3.Controllers;

namespace UnitTests.Features.Students.Controllers;

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
        var expectedResponse = new StudentDto(
                1, _createValidStudentRequest1.Name);

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
            .ThrowsAsync(new MinLengthException(nameof(Student)));
        
        // Act
        var result = await Assert.ThrowsAsync<MinLengthException>(() =>
            _controller.Create(_createInvalidStudentRequest));
        
        // Assert
        Assert.Equal(ReturnMessages.MinLength(nameof(Student)), result.Message);   
    }
    
    [Fact]
    public async Task Create_ShouldReturnError_WhenStudentNameIsNotUnique()
    {
        // Arrange
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<CreateStudentRequest>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new UniqueNameException(nameof(Student), It.IsAny<string>()));
        
        // Act
        var result = await Assert.ThrowsAsync<UniqueNameException>(() =>
            _controller.Create(_createValidStudentRequest1));
        
        // Assert
        Assert.Equal(ReturnMessages.UniqueName(nameof(Student), It.IsAny<string>()), result.Message);   
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
            .ReturnsAsync(new ApiResult<IEnumerable<StudentDto>>(expectedStudents));

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var actualStudents = Assert.IsType<ApiResult<IEnumerable<StudentDto>>>(okResult.Value, exactMatch: false);
        Assert.Equal(expectedStudents.Count, actualStudents.Data.Count());
    }
    
    [Fact]
    public async Task GetAll_ShouldReturnEmptyList_WhenNoStudentExists()
    {
        // Arrange
        var request = new ApiResult<IEnumerable<StudentDto>>(
            new List<StudentDto>(), 
            ReturnMessages.CollectionNotFound(nameof(Student)));
        
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetAllStudentsRequest>(), CancellationToken.None))
            .ReturnsAsync(request);

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var actualStudents = Assert.IsType<ApiResult<IEnumerable<StudentDto>>>(okResult.Value, exactMatch: false);
        
        Assert.Empty(actualStudents.Data.ToList());
        Assert.Equal(actualStudents.Message, ReturnMessages.CollectionNotFound(nameof(Student)));
    }
    
    [Fact]
    public async Task Update_ShouldReturnOk_WhenStudentIsValid()
    {
        // Arrange
        const int studentId = 1;
        const string updatedName = "New Name";
    
        var updateRequest = new UpdateStudentRequest { Name = updatedName };
        
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<UpdateStudentRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new StudentDto(studentId, updatedName));
        
        // Act
        var result = await _controller.Update(studentId, updateRequest);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var actual = Assert.IsType<StudentDto>(okResult.Value);
    
        Assert.Equal(updatedName, actual.Name);
        Assert.Equal(studentId, actual.Id);
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("Az")]
    public async Task Update_ShouldReturnFailure_WhenStudentNameIsInvalid(string? invalidName)
    {
        // Arrange
        const int studentId = 1;

        var updateRequest = new UpdateStudentRequest { Name = invalidName };
        
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<UpdateStudentRequest>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new MinLengthException(nameof(Student)));
        
        // Act
        var result = await Assert.ThrowsAsync<MinLengthException>(() 
            => _controller.Update(studentId, updateRequest));

        // Assert
        Assert.Equal(ReturnMessages.MinLength(nameof(Student)), result.Message);
    }
    
    [Fact]
    public async Task Update_ShouldReturnFailure_WhenStudentIdIsInvalid()
    {
        // Arrange
        const int studentId = 0;

        var updateRequest = new UpdateStudentRequest { Name = _createValidStudentRequest1.Name };
        
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<UpdateStudentRequest>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new EntityNotFoundException(nameof(Student), It.IsAny<int>()));
        
        // Act
        var result = await Assert.ThrowsAsync<EntityNotFoundException>(() 
            => _controller.Update(studentId, updateRequest));

        // Assert
        Assert.Equal(ReturnMessages.EntityNotFound(nameof(Student), studentId), result.Message);
    }
}