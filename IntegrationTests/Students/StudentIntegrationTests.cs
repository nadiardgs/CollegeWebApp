using System.Net;
using Application.Constants;
using Application.Entities.Students.Requests;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;

namespace IntegrationTests.Students;

public class StudentIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public StudentIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
        using var scope = factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<CollegeDbContext>();
        
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
    }
    
    [Fact]
    public async Task Post_Student_ReturnsCreatedStatus()
    {
        // Arrange
        var command = new CreateStudentRequest("John Doe");

        // Act
        var response = await _client.PostAsJsonAsync("/api/students", command);

        // 3. Assert
        response.EnsureSuccessStatusCode(); 
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }
    
    [Fact]
    public async Task CreateStudent_ShouldReturn400_WhenNameIsTooShort()
    {
        // Arrange
        var command = new CreateStudentRequest("Ab");
        
        // Act
        var response = await _client.PostAsJsonAsync("/api/students", command);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        
        var content = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        Assert.NotNull(content);
        
        Assert.Equal(ValidationMessages.ValidationFailed, content.Title);
        Assert.True(content.Errors.ContainsKey("Name"));
        Assert.Contains(ValidationMessages.StudentNameMinLength, content.Errors["Name"]);
    }
}
