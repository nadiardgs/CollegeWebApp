using Application.Validators;
using FluentValidation;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi("v1"); 

builder.Services.AddDbContext<CollegeDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddValidatorsFromAssemblyContaining<StudentValidator>();
builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options => 
    {
        options.WithTitle("College System API")
            .WithTheme(ScalarTheme.Moon);
    });
}

// Seeding logic...
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<CollegeDbContext>();
    await DbSeeder.SeedAsync(context);
}

app.UseHttpsRedirection();
app.MapControllers(); // IMPORTANT: You were missing this call!

app.MapGet("/", () => Results.Redirect("/scalar/v1"));

app.Run();