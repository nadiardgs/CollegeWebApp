using System.Net;
using Application.Features.Teachers.Requests;
using Infrastructure;

namespace IntegrationTests.Teachers;

public class StudentIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public StudentIntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();

        using var scope = factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<CollegeDbContext>();
    
        context.Database.EnsureCreated();
    }

    [Fact]
    public async Task Post_Student_ReturnsCreatedStatus()
    {
        // Arrange
        var command = new CreateTeacherRequest("John Doe");
        
        // Act
        var response = await _client.PostAsJsonAsync("/api/teachers", command);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task CreateStudent_ShouldReturn400_WhenNameIsTooShort()
    {
        // Act
        var command = new CreateTeacherRequest("Ab");
        
        // Arrange
        var response = await _client.PostAsJsonAsync("/api/Teachers", command);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}