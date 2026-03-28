using Application.Responses.Students;
using Application.Responses.Students.DTOs;
using Domain.Entities;
using Infrastructure;
using MediatR;

namespace Application.Requests.Students;

public class CreateStudentRequestHandler(CollegeDbContext context)
    : IRequestHandler<CreateStudentRequest, CreateStudentResponse>
{
    public async Task<CreateStudentResponse> Handle(CreateStudentRequest request, CancellationToken ct)
    {
        var student = new Student() { Name = request.Name };
        
        context.Students.Add(student);
        await context.SaveChangesAsync(ct);

        return new CreateStudentResponse(
            new StudentDto(
                student.Id, 
                student.Name)
        );
    }


}