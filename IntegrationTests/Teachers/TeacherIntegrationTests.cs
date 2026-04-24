using System.Net;
using Application.Constants;
using Application.Features.Teachers.Requests;
using Application.Features.Teachers.Responses;
using Application.Models;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;

namespace IntegrationTests.Teachers;

public class TeacherIntegrationTests(WebApplicationFactory<Program> factory) : IntegrationTestsBase(factory)
{
    private const string RequestUri = "/api/teachers";
    private static readonly TeacherDto ValidTeacher1 = new (1, "John Doe");
    private static readonly TeacherDto ValidTeacher2 = new (2, "Jane Doe");
    private static readonly TeacherDto InvalidTeacher = new (1, "Jo");
    
    [Fact]
    public async Task Create_ShouldReturnCreated_WhenTeacherIsValid()
    {
        // Arrange
        var command = new CreateTeacherRequest(ValidTeacher1.Name);
        
        // Act
        var response = await Client.PostAsJsonAsync(RequestUri, command);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        
        var result = await response.Content.ReadFromJsonAsync<CreateTeacherResponse>();

        Assert.NotNull(result);
        Assert.Equal(result.TeacherDto.Name, ValidTeacher1.Name);
        
        var teacherInDb = await Context.Teachers.FirstOrDefaultAsync(s => s.Name == ValidTeacher1.Name);
        Assert.NotNull(teacherInDb);
    }

    [Fact]
    public async Task Create_ShouldReturn400_WhenNameIsTooShort()
    {
        // Act
        var command = new CreateTeacherRequest(InvalidTeacher.Name);
        
        // Arrange
        var response = await Client.PostAsJsonAsync(RequestUri, command);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        
        var errorResponse = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        
        Assert.NotNull(errorResponse);
        Assert.Equal(errorResponse.Errors["Name"][0], ReturnMessages.MinLength(nameof(Teacher)));
    }
    
    [Fact]
    public async Task Create_ShouldReturnBadRequest_WhenNameAlreadyExists()
    {
        // Arrange
        var createCommand = new CreateTeacherRequest(ValidTeacher1.Name);

        var createResponse = await Client.PostAsJsonAsync(RequestUri, createCommand);
        
        await createResponse.Content.ReadFromJsonAsync<TeacherDto>();

        var errorCommand = new CreateTeacherRequest(ValidTeacher1.Name);
        
        // Act
        var errorResponse = await Client.PostAsJsonAsync(RequestUri, errorCommand);
        
        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, errorResponse.StatusCode);
        
        var response = await errorResponse.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        
        Assert.NotNull(response);
        Assert.Equal(response.Errors["Name"][0], ReturnMessages.UniqueName(nameof(Teacher), ValidTeacher1.Name));
    }
    
    [Fact]
    public async Task Update_ShouldReturnOk_WhenTeacherExists()
    {
        // Arrange
        var teacher = new Teacher
        {
            Id = ValidTeacher1.Id,
            Name = ValidTeacher1.Name
        };

        Context.Teachers.Add(teacher);
        await Context.SaveChangesAsync();

        var updatedTeacher = new UpdateTeacherRequest
        {
            Id = ValidTeacher1.Id,
            Name = ValidTeacher2.Name
        };
        
        // Act
        var response = await Client.PatchAsJsonAsync($"{RequestUri}/{ValidTeacher1.Id}", updatedTeacher);
        
        // Assert
        response.EnsureSuccessStatusCode();
        
        var result = await response.Content.ReadFromJsonAsync<TeacherDto>();
        
        Assert.NotNull(result);
        
        Context.ChangeTracker.Clear();
        var teacherInDb = await Context.Teachers.AsNoTracking().FirstOrDefaultAsync(s => s.Name == ValidTeacher2.Name);
        
        Assert.NotNull(teacherInDb);
        Assert.Equal(ValidTeacher1.Id, teacherInDb.Id);
        Assert.Equal(ValidTeacher2.Name, teacherInDb.Name);
    }
    
    [Fact]
    public async Task Update_ShouldReturnOk_WhenTeacherExistsAndHasSameName()
    {
        // Arrange
        var teacher = new Teacher
        {
            Id = ValidTeacher1.Id,
            Name = ValidTeacher1.Name
        };

        Context.Teachers.Add(teacher);
        await Context.SaveChangesAsync();

        var updatedTeacher = new UpdateTeacherRequest
        {
            Id = teacher.Id,
            Name = teacher.Name
        };
        
        // Act
        var response = await Client.PatchAsJsonAsync($"{RequestUri}/{teacher.Id}", updatedTeacher);
        
        // Assert
        response.EnsureSuccessStatusCode();
        
        var result = await response.Content.ReadFromJsonAsync<TeacherDto>();
        
        Assert.NotNull(result);
        
        Context.ChangeTracker.Clear();
        var teacherInDb = await Context.Teachers.AsNoTracking().FirstOrDefaultAsync(s => s.Id == ValidTeacher1.Id);

        Assert.NotNull(teacherInDb);
        Assert.Equal(ValidTeacher1.Name, teacherInDb.Name);
    }
    
    [Fact]
    public async Task Update_ShouldReturn400_WhenNameIsTooShort()
    {
        // Act
        var teacher = new Teacher
        {
            Id = ValidTeacher1.Id,
            Name = ValidTeacher1.Name
        };

        Context.Teachers.Add(teacher);
        await Context.SaveChangesAsync();

        var command = new UpdateTeacherRequest
        {
            Id = teacher.Id,
            Name = InvalidTeacher.Name
        };
        
        // Arrange
        var response = await Client.PatchAsJsonAsync($"{RequestUri}/{teacher.Id}", command);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        
        var errorResponse = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        
        Assert.NotNull(errorResponse);
        Assert.Equal(errorResponse.Errors["Name"][0], ReturnMessages.MinLength(nameof(Teacher)));
    }
    
    [Fact]
    public async Task Update_ShouldReturnBadRequest_WhenTeacherDoesntExist()
    {
        // Arrange
        var invalidId = 999;
        
        var teacher = new Teacher
        {
            Id = ValidTeacher1.Id,
            Name = ValidTeacher1.Name
        };

        Context.Teachers.Add(teacher);
        await Context.SaveChangesAsync();

        var updatedTeacher = new UpdateTeacherRequest
        {
            Id = invalidId,
            Name = teacher.Name
        };
        
        // Act
        var response = await Client.PatchAsJsonAsync($"{RequestUri}/{invalidId}", updatedTeacher);
        
        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        
        var errorResponse = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        
        Assert.NotNull(errorResponse);
        Assert.Equal(errorResponse.Errors["Id"][0], ReturnMessages.EntityNotFound(nameof(Teacher), invalidId));
    }
    
    [Fact]
    public async Task GetAll_ShouldReturnOk_WithData_WhenTeachersExist()
    {
        // Arrange
        var teachers = new List<Teacher>
        {
            new() { Name = ValidTeacher1.Name },
            new() { Name = ValidTeacher2.Name }
        };
        
        Context.Teachers.AddRange(teachers);
        await Context.SaveChangesAsync();

        // Act
        var response = await Client.GetAsync(RequestUri);

        // Assert
        response.EnsureSuccessStatusCode();
        
        var result = await response.Content.ReadFromJsonAsync<ApiResult<IEnumerable<TeacherDto>>>();
        
        Assert.NotNull(result);
        Assert.Equal(teachers.Count, result.Data.Count());
        Assert.Contains(result.Data, s => s.Name == ValidTeacher1.Name);
        Assert.Contains(result.Data, s => s.Name == ValidTeacher2.Name);
        Assert.Equal(ReturnMessages.Success(teachers.Count, nameof(Teacher)), result.Message);
    }
    
    [Fact]
    public async Task GetAll_ShouldReturnOk_WithEmptyList_WhenNoTeachersExist()
    {
        // Act
        var response = await Client.GetAsync(RequestUri);

        // Assert
        response.EnsureSuccessStatusCode();
        
        var result = await response.Content.ReadFromJsonAsync<ApiResult<IEnumerable<TeacherDto>>>();
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(result);
        Assert.Empty(result.Data);
        Assert.Equal(ReturnMessages.CollectionNotFound(nameof(Teacher)), result.Message);
    }
}