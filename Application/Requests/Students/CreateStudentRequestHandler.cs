using Application.Requests.Teachers;
using Application.Responses.Students;
using Application.Responses.Teachers;
using Domain.Entities;
using Infrastructure;
using MediatR;

namespace Application.Requests.Students;

public class CreateStudentRequestHandler(CollegeDbContext context)
    : IRequestHandler<CreateStudentRequest, CreateStudentResponse>
{
    public async Task<CreateStudentResponse> Handle(CreateStudentRequest request, CancellationToken ct)
    {
        var teacher = new Teacher { Name = request.Name };
        
        context.Teachers.Add(teacher);
        await context.SaveChangesAsync(ct);

        return new CreateStudentResponse(teacher.Id, teacher.Name);
    }


}