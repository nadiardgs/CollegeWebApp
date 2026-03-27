using Application.Responses;
using Application.Responses.Teachers;
using Domain.Entities;
using Infrastructure;
using MediatR;

namespace Application.Requests.Teachers;

public class CreateTeacherRequestHandler(CollegeDbContext context)
    : IRequestHandler<CreateTeacherRequest, CreateTeacherResponse>
{
    public async Task<CreateTeacherResponse> Handle(CreateTeacherRequest request, CancellationToken ct)
    {
        var teacher = new Teacher { Name = request.Name };
        
        context.Teachers.Add(teacher);
        await context.SaveChangesAsync(ct);

        return new CreateTeacherResponse(teacher.Id, teacher.Name);
    }
}