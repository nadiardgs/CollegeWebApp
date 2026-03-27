using Application.Requests.Teachers;
using FluentValidation;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using WebApplication3;

var builder = WebApplication.CreateBuilder(args);

var applicationAssembly = typeof(CreateTeacherRequest).Assembly;

builder.Services.AddValidatorsFromAssembly(applicationAssembly);

builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssembly(applicationAssembly);
    cfg.AddOpenBehavior(typeof(Application.Behaviors.ValidationBehavior<,>));
});

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails(); 

builder.Services.AddDbContext<CollegeDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();
builder.Services.AddOpenApi("v1"); 

var app = builder.Build();

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

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<CollegeDbContext>();
    await DbSeeder.SeedAsync(context);
}

app.UseHttpsRedirection();
app.MapControllers(); 

app.MapGet("/", () => Results.Redirect("/scalar/v1"));

app.Run();