using Infrastructure;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace IntegrationTests;

public abstract class IntegrationTestsBase : IClassFixture<WebApplicationFactory<Program>>
{
    protected readonly HttpClient Client;
    protected readonly CollegeDbContext Context;
    private readonly IServiceScope _scope;

    protected IntegrationTestsBase(WebApplicationFactory<Program> factory)
    {
        var dbName = $"DataSource=file:{Guid.NewGuid()}?mode=memory&cache=shared";
        var connection = new SqliteConnection(dbName);
        connection.Open();

        var testFactory = factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Testing");
            
            builder.ConfigureServices(services =>
            {
                var descriptors = services.Where(d => 
                        d.ServiceType.Name.Contains("DbContextOptions") || 
                        d.ServiceType == typeof(CollegeDbContext))
                    .ToList();

                foreach (var descriptor in descriptors)
                {
                    services.Remove(descriptor);
                }
                
                services.AddDbContext<CollegeDbContext>(options =>
                {
                    options.UseSqlite(connection);
                    options.UseInternalServiceProvider(null); 
                });
            });
        });

        _scope = testFactory.Services.CreateScope();
        Context = _scope.ServiceProvider.GetRequiredService<CollegeDbContext>();
        Client = testFactory.CreateClient();

        Context.Database.EnsureCreated();
    }
}