using Application.Constants;
using Application.Exceptions;
using Application.Features.Courses.Requests;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using UnitTests.TestBases.Teachers;

namespace UnitTests.Features.Courses.Handlers;

public class EnrollTeacherInCourseHandlerTests : TeacherTestBase
{
    private readonly EnrollTeacherInCourseRequestHandler _handler;

    public EnrollTeacherInCourseHandlerTests()
    {
        _handler = new EnrollTeacherInCourseRequestHandler(Context);
    }

    [Fact]
    public async Task Handler_ShouldBeValid_WhenCourseHasNoTeacher()
    {
        // Arrange
        SeedCourse(
            ValidCourse.Id, 
            ValidCourse.Title);
        
        SeedTeacher(
            ValidTeacher1.Id, 
            ValidTeacher1.Name);
        
        await Context.SaveChangesAsync();
        
        var enrollmentRequest = new EnrollTeacherInCourseRequest(ValidCourse.Id, ValidTeacher1.Id);
        
        // Act
        var result = await _handler.Handle(enrollmentRequest, CancellationToken.None);
        
        // Assert
        var updatedCourse = await Context.Courses.FirstAsync(c => c.Id == ValidCourse.Id);
        
        Assert.Equal(ValidTeacher1.Id, updatedCourse.TeacherId);
        Assert.Equal(ValidCourse.Id, updatedCourse.Id);
        Assert.Equal(ValidCourse.Title, updatedCourse.Title);
        
        Assert.True(result.Success);
        Assert.Equal(1, await Context.Teachers.CountAsync());
        Assert.Equal(1, await Context.Courses.CountAsync());
    }

    [Fact]
    public async Task Handler_ShouldBeValid_WhenEnrollingTheSameTeacher()
    {
        // Arrange
        SeedCourseWithTeacher(
            ValidCourse.Id,
            ValidCourse.Title,
            ValidTeacher1.Id,
            ValidTeacher1.Name);
        
        await Context.SaveChangesAsync();
        
        var enrollmentRequest = new EnrollTeacherInCourseRequest(ValidCourse.Id, ValidTeacher1.Id);
        
        // Act
        var result = await _handler.Handle(enrollmentRequest, CancellationToken.None);
        
        // Assert
        var updatedCourse = await Context.Courses.FirstAsync(c => c.Id == ValidCourse.Id);
        
        Assert.Equal(ValidTeacher1.Id, updatedCourse.TeacherId);
        Assert.Equal(ValidCourse.Id, updatedCourse.Id);
        Assert.Equal(ValidCourse.Id, updatedCourse.Id);
        Assert.Equal(ValidCourse.Title, updatedCourse.Title);
        
        Assert.True(result.Success);
        Assert.Equal(1, await Context.Teachers.CountAsync());
        Assert.Equal(1, await Context.Courses.CountAsync());
    }
    
    [Fact]
    public async Task Handler_ShouldBeInvalid_WhenCourseHasATeacher()
    {
        // Arrange
        SeedCourseWithTeacher(
            ValidCourse.Id,
            ValidCourse.Title,
            ValidTeacher1.Id,
            ValidTeacher1.Name);
        
        SeedTeacher(
            ValidTeacher2.Id, 
            ValidTeacher2.Name);
        
        await Context.SaveChangesAsync();
        
        var enrollmentRequest = new EnrollTeacherInCourseRequest(ValidCourse.Id, ValidTeacher2.Id);
        
        // Act
        var result = await Assert.ThrowsAsync<TeacherAlreadyAssignedException>(() => 
            _handler.Handle(enrollmentRequest, CancellationToken.None));
        
        // Assert
        Assert.Equal(result.Message, ReturnMessages.TeacherAlreadyAssigned(ValidCourse.Id));
        Assert.Equal(1, await Context.Courses.CountAsync());
    }

    [Fact]
    public async Task Handler_ShouldBeInvalid_WhenCourseDoesntExist()
    {
        // Arrange
        SeedTeacher(
            ValidTeacher1.Id,
            ValidTeacher1.Name);
        
        await Context.SaveChangesAsync();
        
        // Act
        var enrollmentRequest = new EnrollTeacherInCourseRequest(ValidCourse.Id, ValidTeacher1.Id);

        // Act
        var result = await Assert.ThrowsAsync<EntityNotFoundException>(() => 
            _handler.Handle(enrollmentRequest, CancellationToken.None));
        
        // Assert
        Assert.Equal(result.Message, ReturnMessages.EntityNotFound(nameof(Course), ValidCourse.Id));
        Assert.Equal(0, await Context.Courses.CountAsync());
    }

    [Fact]
    public async Task Handler_ShouldBeInvalid_WhenTeacherDoesntExist()
    {
        // Arrange
        SeedCourse(
            ValidCourse.Id,
            ValidCourse.Title);

        await Context.SaveChangesAsync();

        // Act
        var enrollmentRequest = new EnrollTeacherInCourseRequest(ValidCourse.Id, ValidTeacher1.Id);
        
        // Act
        var result = await Assert.ThrowsAsync<EntityNotFoundException>(() => 
            _handler.Handle(enrollmentRequest, CancellationToken.None));
        
        // Assert
        Assert.Equal(result.Message, ReturnMessages.EntityNotFound(nameof(Teacher), ValidTeacher1.Id));
        Assert.Null((await Context.Courses.FirstAsync()).TeacherId);
    }
}