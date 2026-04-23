using System.Net;
using Application.Constants;
using Application.Features.Students.Requests;
using Application.Features.Students.Responses;
using Application.Models;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc.Testing;

namespace IntegrationTests.Students;

public class StudentIntegrationTests(WebApplicationFactory<Program> factory) : IntegrationTestsBase(factory)
{
    private const string RequestUri = "/api/students";
    private static readonly StudentDto ValidStudent1 = new (1, "John Doe");
    private static readonly StudentDto ValidStudent2 = new (1, "Jane Doe");
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
        Assert.Empty(result.Data);
        Assert.Equal(ReturnMessages.CollectionNotFound(nameof(Student)), result.Message);
    }
}