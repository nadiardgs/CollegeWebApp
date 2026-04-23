using Application.Features.Teachers.Requests;
using FluentValidation;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using WebApplication3;
using WebApplication3.Middleware;

var builder = WebApplication.CreateBuilder(args);

var applicationAssembly = typeof(CreateTeacherRequest).Assembly;

builder.Services.AddValidatorsFromAssembly(applicationAssembly);

builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssembly(applicationAssembly);
    cfg.AddOpenBehavior(typeof(Application.Behaviors.ValidationBehavior<,>));
});

builder.Services.AddDbContext<CollegeDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails(); 
builder.Services.AddControllers();
builder.Services.AddOpenApi("v1"); 

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

app.UseExceptionHandler(); 

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options => 
    {
        options.WithTitle("College System API")
            .WithTheme(ScalarTheme.Moon);
    });
}

if (!app.Environment.IsEnvironment("Testing"))
{
    app.UseHttpsRedirection();
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<CollegeDbContext>();
    await DbSeeder.SeedAsync(context);
}

app.MapControllers(); 

app.MapGet("/", () => Results.Redirect("/scalar/v1"));

app.Run();

public partial class Program { }