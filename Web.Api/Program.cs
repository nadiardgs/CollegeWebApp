using Application.Requests.Grades;
using Application.Requests.Students;
using Application.Requests.Teachers;
using Application.Validators;
using FluentValidation;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi("v1"); 

builder.Services.AddDbContext<CollegeDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddValidatorsFromAssemblyContaining<GradeValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<StudentValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<TeacherValidator>();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateGradeRequest).Assembly));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateStudentRequest).Assembly));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateTeacherRequest).Assembly));

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

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<CollegeDbContext>();
    await DbSeeder.SeedAsync(context);
}

app.UseHttpsRedirection();
app.MapControllers(); 

app.MapGet("/", () => Results.Redirect("/scalar/v1"));

app.Run();