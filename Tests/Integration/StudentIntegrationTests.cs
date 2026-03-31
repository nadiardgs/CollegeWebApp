using System.Net;
using System.Net.Http.Json;
using Application.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Tests.Integration;

public class StudentIntegrationTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task CreateStudent_ShouldReturn400_WhenNameIsTooShort()
    {
        // Act
        var response = await _client.PostAsJsonAsync("/api/students", new { Name = "Ab" });

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        
        var content = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        
        Assert.NotNull(content);
        Assert.Equal(ValidationMessages.StudentNameMinLength, content.Title);
    }
}