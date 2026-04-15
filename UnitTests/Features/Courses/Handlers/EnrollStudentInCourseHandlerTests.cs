using Application.Constants;
using Application.Exceptions;
using Application.Features.Courses.Requests;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using UnitTests.TestBases.Students;

namespace UnitTests.Features.Courses.Handlers;

public class EnrollStudentInCourseHandlerTests : StudentTestBase
{
    private readonly EnrollStudentInCourseRequestHandler _handler;

    public EnrollStudentInCourseHandlerTests()
    {
        _handler = new EnrollStudentInCourseRequestHandler(Context);
    }

    [Fact]
    public async Task Handler_ShouldBeValid_WhenTeacherAndStudentExist()
    {
        // Arrange
        SeedStudent(
            ValidStudent1.Id, 
            ValidStudent1.Name);
            
        SeedCourseWithTeacher(
            ValidCourse.Id, 
            ValidCourse.Title, 
            ValidTeacher.Id, 
            ValidTeacher.Name);
            
        await Context.SaveChangesAsync();

        var enrollment = new EnrollStudentInCourseRequest(ValidCourse.Id, ValidStudent1.Id);
        
        // Act
        var result = await _handler.Handle(enrollment, CancellationToken.None);
        
        // Assert
        Assert.Equal(ValidStudent1.Id, result.StudentId);
        Assert.Equal(ValidStudent1.Name, result.StudentName);
        Assert.Equal(ValidCourse.Id, result.CourseId);
        Assert.Equal(ValidCourse.Title, result.CourseTitle);
        Assert.Equal(1, await Context.Enrollments.CountAsync());
    }

    [Fact]
    public async Task Handler_ShouldBeInvalid_WhenTeacherNotAssigned()
    {
        // Arrange
        SeedStudent(
            ValidStudent1.Id,
            ValidStudent1.Name);
        
        SeedCourse(
            ValidCourse.Id,
            ValidCourse.Title);

        await Context.SaveChangesAsync();

        var enrollmentRequest = new EnrollStudentInCourseRequest(ValidCourse.Id, ValidStudent1.Id);
        
        // Act
        var result = await Assert.ThrowsAsync<NoTeacherAssignedException>(() => 
                    _handler.Handle(enrollmentRequest, CancellationToken.None));
        
        // Assert
        Assert.Equal(result.Message, ReturnMessages.NoTeacherAssigned(ValidCourse.Id));
        Assert.False(Context.Enrollments.Any());
    }

    [Fact]
    public async Task Handler_ShouldBeInvalid_WhenCourseDoesntExist()
    {
        // Arrange
        SeedStudent(
            ValidStudent1.Id,
            ValidStudent1.Name);

        SeedTeacher(
            ValidTeacher.Id,
            ValidTeacher.Name);

        await Context.SaveChangesAsync();

        var enrollmentRequest = new EnrollStudentInCourseRequest(ValidCourse.Id, ValidStudent1.Id);
        
        // Act
        var result = await Assert.ThrowsAsync<EntityNotFoundException>(() 
            => _handler.Handle(enrollmentRequest, CancellationToken.None));
        
        // Assert
        Assert.Equal(result.Message, ReturnMessages.EntityNotFound(nameof(Course), ValidCourse.Id));
        Assert.False(Context.Enrollments.Any());
    }

    [Fact]
    public async Task Handle_ShouldBeInvalid_whenStudentDoesntExist()
    {
        // Arrange
        SeedCourseWithTeacher(
            ValidCourse.Id,
            ValidCourse.Title,
            ValidTeacher.Id,
            ValidTeacher.Name);

        await Context.SaveChangesAsync();

        var enrollmentRequest = new EnrollStudentInCourseRequest(ValidCourse.Id, ValidStudent1.Id);

        // Act
        var result = await Assert.ThrowsAsync<EntityNotFoundException>(() => 
            _handler.Handle(enrollmentRequest, CancellationToken.None));
        
        Assert.Equal(result.Message, ReturnMessages.EntityNotFound(nameof(Student),  ValidStudent1.Id));
        Assert.False(Context.Enrollments.Any());
    }

    [Fact]
    public async Task Handler_ShouldBeInvalid_WhenStudentAlreadyEnrolled()
    {
        // Arrange
        SeedStudent(
            ValidStudent1.Id, 
            ValidStudent1.Name);
        
        SeedCourseWithTeacher(
            ValidCourse.Id, 
            ValidCourse.Title, 
            ValidTeacher.Id, 
            ValidTeacher.Name);
    
        SeedEnrollment(
            ValidStudent1.Id, 
            ValidCourse.Id);

        await Context.SaveChangesAsync();

        var enrollmentRequest = new EnrollStudentInCourseRequest(ValidCourse.Id, ValidStudent1.Id);
    
        // Act
        var result = await Assert.ThrowsAsync<StudentAlreadyEnrolledException>(() =>
            _handler.Handle(enrollmentRequest, CancellationToken.None));
        
        // Assert
        Assert.Equal(result.Message, ReturnMessages.AlreadyEnrolled(ValidStudent1.Id, ValidCourse.Id));
        Assert.Equal(1, await Context.Enrollments.CountAsync());
    }
}