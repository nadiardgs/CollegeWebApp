using Application.Exceptions;
using Application.Features.Courses.Requests;
using Application.Features.Courses.Responses;
using Application.Features.Students.Responses;
using Application.Models;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using UnitTests.TestBases.Teachers;
using WebApplication3.Controllers;

namespace UnitTests.Features.Courses.Controllers;

public class CoursesControllerTests : TeacherTestBase
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly CoursesController _controller;
    private readonly CreateCourseRequest _createValidCourseRequest1;
    private readonly CreateCourseRequest _createValidCourseRequest2;
    private readonly StudentEnrollmentDto _studentEnrollmentDto;
    private readonly StudentDto _studentDto;
    private readonly CourseDto _courseDto;
    
    public CoursesControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new CoursesController(_mediatorMock.Object);
        _createValidCourseRequest1 = new CreateCourseRequest("Course Test 1");
        _createValidCourseRequest2 = new CreateCourseRequest("Course Test 2");
        _courseDto = new CourseDto(1, "Test Course");
        _studentDto = new StudentDto(1, "Test Student");
        _studentEnrollmentDto = new StudentEnrollmentDto(
            1, _studentDto.Id, _studentDto.Name, _courseDto.Id, _courseDto.Title);
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
    
    [Fact]
    public async Task EnrollStudent_ShouldReturnOk_WhenCourseAndTeacherExist()
    {
        // Arrange
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<EnrollStudentInCourseRequest>(), CancellationToken.None))
            .ReturnsAsync(_studentEnrollmentDto);
        
        // Act
        var result = await _controller.EnrollStudent(_studentEnrollmentDto.CourseId, _studentEnrollmentDto.StudentId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var enrollment = Assert.IsType<StudentEnrollmentDto>(okResult.Value, exactMatch: true);
        
        Assert.Equal(_studentEnrollmentDto.EnrollmentId, enrollment.EnrollmentId);
        Assert.Equal(_studentEnrollmentDto.CourseId, enrollment.CourseId);
        Assert.Equal(_studentEnrollmentDto.CourseTitle, enrollment.CourseTitle);
        Assert.Equal(_studentEnrollmentDto.StudentId, enrollment.StudentId);
        Assert.Equal(_studentEnrollmentDto.StudentName, enrollment.StudentName);
    }
    
    [Fact]
    public async Task EnrollStudent_ShouldThrowException_WhenCourseHasNoTeacher()
    {
        // Arrange
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<EnrollStudentInCourseRequest>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NoTeacherAssignedException(_courseDto.Id));

        // Act
        var result = await Assert.ThrowsAsync<NoTeacherAssignedException>(() => 
            _controller.EnrollStudent(_courseDto.Id, _studentDto.Id));
        Assert.Equal(_courseDto.Id, result.CourseId);
        
        // Assert
        _mediatorMock.Verify(m => m.Send(
                It.Is<EnrollStudentInCourseRequest>(req => 
                    req.CourseId == _courseDto.Id && 
                    req.StudentId == _studentDto.Id), 
                It.IsAny<CancellationToken>()), 
            Times.Once);
    }
    
    [Fact]
    public async Task EnrollStudent_ShouldThrowException_WhenCourseDoesntExist()
    {
        // Arrange
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<EnrollStudentInCourseRequest>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new EntityNotFoundException(nameof(Course), _courseDto.Id));

        // Act
        await Assert.ThrowsAsync<EntityNotFoundException>(() => 
            _controller.EnrollStudent(_courseDto.Id, _studentDto.Id));
        
        // Assert
        _mediatorMock.Verify(m => m.Send(
                It.Is<EnrollStudentInCourseRequest>(req => 
                    req.CourseId == _courseDto.Id && 
                    req.StudentId == _studentDto.Id), 
                It.IsAny<CancellationToken>()), 
            Times.Once);
        
        _mediatorMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task EnrollStudent_ShouldThrowException_WhenTeacherDoesntExist()
    {
        // Arrange
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<EnrollStudentInCourseRequest>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new EntityNotFoundException(nameof(Course), _courseDto.Id));

        // Act
        await Assert.ThrowsAsync<EntityNotFoundException>(() => 
            _controller.EnrollStudent(_courseDto.Id, _studentDto.Id));
        
        // Assert
        _mediatorMock.Verify(m => m.Send(
                It.Is<EnrollStudentInCourseRequest>(req => 
                    req.CourseId == _courseDto.Id && 
                    req.StudentId == _studentDto.Id), 
                It.IsAny<CancellationToken>()), 
            Times.Once);
        
        _mediatorMock.VerifyNoOtherCalls();
    }
    
    [Fact]
    public async Task EnrollStudent_ShouldThrowException_WhenStudentDoesntExist()
    {
        // Arrange
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<EnrollStudentInCourseRequest>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new EntityNotFoundException(nameof(Student), _studentDto.Id));

        // Act
        await Assert.ThrowsAsync<EntityNotFoundException>(() => 
            _controller.EnrollStudent(_courseDto.Id, _studentDto.Id));
        
        // Assert
        _mediatorMock.Verify(m => m.Send(
                It.Is<EnrollStudentInCourseRequest>(req => 
                    req.CourseId == _courseDto.Id && 
                    req.StudentId == _studentDto.Id), 
                It.IsAny<CancellationToken>()), 
            Times.Once);
        
        _mediatorMock.VerifyNoOtherCalls();
    }
    
    [Fact]
    public async Task EnrollStudent_ShouldThrowException_WhenStudentAlreadyEnrolled()
    {
        // Arrange
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<EnrollStudentInCourseRequest>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new StudentAlreadyEnrolledException(_studentDto.Id, _courseDto.Id));

        // Act
        await Assert.ThrowsAsync<StudentAlreadyEnrolledException>(() => 
            _controller.EnrollStudent(_courseDto.Id, _studentDto.Id));
        
        // Assert
        _mediatorMock.Verify(m => m.Send(
                It.Is<EnrollStudentInCourseRequest>(req => 
                    req.CourseId == _courseDto.Id && 
                    req.StudentId == _studentDto.Id), 
                It.IsAny<CancellationToken>()), 
            Times.Once);
        
        _mediatorMock.VerifyNoOtherCalls();
    }
}