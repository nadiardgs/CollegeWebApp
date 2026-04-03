using System.Net;
using Application.Entities.Students.Requests;
using Infrastructure;

namespace IntegrationTests.Students;

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
        var command = new CreateStudentRequest("John Doe " + Guid.NewGuid());
        var response = await _client.PostAsJsonAsync("/api/students", command);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task CreateStudent_ShouldReturn400_WhenNameIsTooShort()
    {
        var command = new CreateStudentRequest("Ab");
        var response = await _client.PostAsJsonAsync("/api/students", command);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}