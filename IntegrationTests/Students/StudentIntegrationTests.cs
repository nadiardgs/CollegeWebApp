using System.Net;
using Application.Constants;
using Application.Features.Students.Requests;
using Application.Features.Students.Responses;
using Application.Models;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;

namespace IntegrationTests.Students;

public class StudentIntegrationTests(WebApplicationFactory<Program> factory) : IntegrationTestsBase(factory)
{
    private const string RequestUri = "/api/students";
    private static readonly StudentDto ValidStudent1 = new (1, "John Doe");
    private static readonly StudentDto ValidStudent2 = new (2, "Jane Doe");
    private static readonly StudentDto InvalidStudent = new (1, "Jo");

    [Fact]
    public async Task Create_ShouldReturnCreated_WhenStudentIsValid()
    {
        // Arrange
        var command = new CreateStudentRequest(ValidStudent1.Name);
        
        // Act
        var response = await Client.PostAsJsonAsync(RequestUri, command);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        
        var result = await response.Content.ReadFromJsonAsync<StudentDto>();

        Assert.NotNull(result);
        Assert.Equal(result.Name, ValidStudent1.Name);
        
        var studentInDb = await Context.Students.FirstOrDefaultAsync(s => s.Name == ValidStudent1.Name);
        Assert.NotNull(studentInDb);
    }

    [Fact]
    public async Task Create_ShouldReturn400_WhenNameIsTooShort()
    {
        // Act
        var command = new CreateStudentRequest(InvalidStudent.Name);
        
        // Arrange
        var response = await Client.PostAsJsonAsync(RequestUri, command);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        
        var errorResponse = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.NotNull(errorResponse);
        
        Assert.NotNull(response);
        Assert.Equal(ReturnMessages.MinLength(nameof(Student)), errorResponse.Detail);
    }
    
    [Fact]
    public async Task Create_ShouldReturnConflict_WhenNameAlreadyExists()
    {
        // Arrange
        var createCommand = new CreateStudentRequest(ValidStudent1.Name);

        var createResponse = await Client.PostAsJsonAsync(RequestUri, createCommand);
        
        await createResponse.Content.ReadFromJsonAsync<StudentDto>();

        var errorCommand = new CreateStudentRequest(ValidStudent1.Name);
        
        // Act
        var errorResponse = await Client.PostAsJsonAsync(RequestUri, errorCommand);
        
        // Assert
        Assert.Equal(HttpStatusCode.Conflict, errorResponse.StatusCode);
        
        var response = await errorResponse.Content.ReadFromJsonAsync<ProblemDetails>();
    
        Assert.NotNull(response);
        Assert.Equal(ReturnMessages.UniqueName(nameof(Student), ValidStudent1.Name), response.Detail);
    }

    [Fact]
    public async Task Update_ShouldReturnOk_WhenStudentExists()
    {
        // Arrange
        var student = new Student
        {
            Id = ValidStudent1.Id,
            Name = ValidStudent1.Name
        };

        Context.Students.Add(student);
        await Context.SaveChangesAsync();

        var updatedStudent = new UpdateStudentRequest
        {
            Id = ValidStudent1.Id,
            Name = ValidStudent2.Name
        };
        
        // Act
        var response = await Client.PatchAsJsonAsync($"{RequestUri}/{ValidStudent1.Id}", updatedStudent);
        
        // Assert
        response.EnsureSuccessStatusCode();
        
        var result = await response.Content.ReadFromJsonAsync<StudentDto>();
        
        Assert.NotNull(result);
        
        Context.ChangeTracker.Clear();
        var studentInDb = await Context.Students.AsNoTracking().FirstOrDefaultAsync(s => s.Name == ValidStudent2.Name);
        
        Assert.NotNull(studentInDb);
        Assert.Equal(ValidStudent1.Id, studentInDb.Id);
        Assert.Equal(ValidStudent2.Name, studentInDb.Name);
    }
    
    [Fact]
    public async Task Update_ShouldReturnOk_WhenStudentExistsAndHasSameName()
    {
        // Arrange
        var student = new Student
        {
            Id = ValidStudent1.Id,
            Name = ValidStudent1.Name
        };

        Context.Students.Add(student);
        await Context.SaveChangesAsync();

        var updatedStudent = new UpdateStudentRequest
        {
            Id = student.Id,
            Name = student.Name
        };
        
        // Act
        var response = await Client.PatchAsJsonAsync($"{RequestUri}/{student.Id}", updatedStudent);
        
        // Assert
        response.EnsureSuccessStatusCode();
        
        var result = await response.Content.ReadFromJsonAsync<StudentDto>();
        
        Assert.NotNull(result);
        
        Context.ChangeTracker.Clear();
        var studentInDb = await Context.Students.AsNoTracking().FirstOrDefaultAsync(s => s.Id == ValidStudent1.Id);

        Assert.NotNull(studentInDb);
        Assert.Equal(ValidStudent1.Name, studentInDb.Name);
    }

    [Fact]
    public async Task Update_ShouldReturn400_WhenNameIsTooShort()
    {
        // Act
        var student = new Student
        {
            Id = ValidStudent1.Id,
            Name = ValidStudent1.Name
        };

        Context.Students.Add(student);
        await Context.SaveChangesAsync();

        var command = new UpdateStudentRequest
        {
            Id = student.Id,
            Name = InvalidStudent.Name
        };
        
        // Arrange
        var response = await Client.PatchAsJsonAsync($"{RequestUri}/{student.Id}", command);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        
        var errorResponse = await response.Content.ReadFromJsonAsync<ProblemDetails>();
    
        Assert.NotNull(errorResponse);
        Assert.Equal(ReturnMessages.MinLength(nameof(Student)), errorResponse.Detail);
    }
    
    [Fact]
    public async Task Update_ShouldReturnNotFound_WhenStudentDoesntExist()
    {
        // Arrange
        var invalidId = 999;
        
        var student = new Student
        {
            Id = ValidStudent1.Id,
            Name = ValidStudent1.Name
        };

        Context.Students.Add(student);
        await Context.SaveChangesAsync();

        var updatedStudent = new UpdateStudentRequest
        {
            Id = invalidId,
            Name = student.Name
        };
        
        // Act
        var response = await Client.PatchAsJsonAsync($"{RequestUri}/{invalidId}", updatedStudent);
        
        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        
        var errorResponse = await response.Content.ReadFromJsonAsync<ProblemDetails>();

        Assert.NotNull(errorResponse);
        Assert.Equal(ReturnMessages.EntityNotFound(nameof(Student), invalidId), errorResponse.Detail);
    }
    
    [Fact]
    public async Task GetAll_ShouldReturnOk_WithData_WhenStudentsExist()
    {
        // Arrange
        var students = new List<Student>
        {
            new() { Name = ValidStudent1.Name },
            new() { Name = ValidStudent2.Name }
        };
        
        Context.Students.AddRange(students);
        await Context.SaveChangesAsync();

        // Act
        var response = await Client.GetAsync(RequestUri);

        // Assert
        response.EnsureSuccessStatusCode();
        
        var result = await response.Content.ReadFromJsonAsync<ApiResult<IEnumerable<StudentDto>>>();
        
        Assert.NotNull(result);
        Assert.Equal(students.Count, result.Data.Count());
        Assert.Contains(result.Data, s => s.Name == ValidStudent1.Name);
        Assert.Contains(result.Data, s => s.Name == ValidStudent2.Name);
        Assert.Equal(ReturnMessages.Success(students.Count, nameof(Student)), result.Message);
    }
    
    [Fact]
    public async Task GetAll_ShouldReturnOk_WithEmptyList_WhenNoStudentsExist()
    {
        // Act
        var response = await Client.GetAsync(RequestUri);

        // Assert
        response.EnsureSuccessStatusCode();
        
        var result = await response.Content.ReadFromJsonAsync<ApiResult<IEnumerable<StudentDto>>>();
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(result);
        Assert.Empty(result.Data);
        Assert.Equal(ReturnMessages.CollectionNotFound(nameof(Student)), result.Message);
    }
}