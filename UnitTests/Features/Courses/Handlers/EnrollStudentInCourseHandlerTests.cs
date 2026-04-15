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
            ValidStudent.Id, 
            ValidStudent.Name);
            
        SeedCourseWithTeacher(
            ValidCourse.Id, 
            ValidCourse.Title, 
            ValidTeacher.Id, 
            ValidTeacher.Name);
            
        await Context.SaveChangesAsync();

        var enrollment = new EnrollStudentInCourseRequest(ValidCourse.Id, ValidStudent.Id);
        
        // Act
        var result = await _handler.Handle(enrollment, CancellationToken.None);
        
        // Assert
        Assert.Equal(ValidStudent.Id, result.StudentId);
        Assert.Equal(ValidStudent.Name, result.StudentName);
        Assert.Equal(ValidCourse.Id, result.CourseId);
        Assert.Equal(ValidCourse.Title, result.CourseTitle);
        Assert.Equal(1, await Context.Enrollments.CountAsync());
    }

    [Fact]
    public async Task Handler_ShouldBeInvalid_WhenTeacherNotAssigned()
    {
        // Arrange
        SeedStudent(
            ValidStudent.Id,
            ValidStudent.Name);
        
        SeedCourse(
            ValidCourse.Id,
            ValidCourse.Title);

        await Context.SaveChangesAsync();

        var enrollmentRequest = new EnrollStudentInCourseRequest(ValidCourse.Id, ValidStudent.Id);
        
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
            ValidStudent.Id,
            ValidStudent.Name);

        SeedTeacher(
            ValidTeacher.Id,
            ValidTeacher.Name);

        await Context.SaveChangesAsync();

        var enrollmentRequest = new EnrollStudentInCourseRequest(ValidCourse.Id, ValidStudent.Id);
        
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

        var enrollmentRequest = new EnrollStudentInCourseRequest(ValidCourse.Id, ValidStudent.Id);

        // Act
        var result = await Assert.ThrowsAsync<EntityNotFoundException>(() => 
            _handler.Handle(enrollmentRequest, CancellationToken.None));
        
        Assert.Equal(result.Message, ReturnMessages.EntityNotFound(nameof(Student),  ValidStudent.Id));
        Assert.False(Context.Enrollments.Any());
    }

    [Fact]
    public async Task Handler_ShouldBeInvalid_WhenStudentAlreadyEnrolled()
    {
        // Arrange
        SeedStudent(
            ValidStudent.Id, 
            ValidStudent.Name);
        
        SeedCourseWithTeacher(
            ValidCourse.Id, 
            ValidCourse.Title, 
            ValidTeacher.Id, 
            ValidTeacher.Name);
    
        SeedEnrollment(
            ValidStudent.Id, 
            ValidCourse.Id);

        await Context.SaveChangesAsync();

        var enrollmentRequest = new EnrollStudentInCourseRequest(ValidCourse.Id, ValidStudent.Id);
    
        // Act
        var result = await Assert.ThrowsAsync<StudentAlreadyEnrolledException>(() =>
            _handler.Handle(enrollmentRequest, CancellationToken.None));
        
        // Assert
        Assert.Equal(result.Message, ReturnMessages.AlreadyEnrolled(ValidStudent.Id, ValidCourse.Id));
        Assert.Equal(1, await Context.Enrollments.CountAsync());
    }
}