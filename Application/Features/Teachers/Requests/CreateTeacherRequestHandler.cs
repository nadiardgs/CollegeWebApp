using Application.Exceptions;
using Application.Features.Teachers.Responses;
using Domain.Entities;
using Infrastructure;
using Infrastructure.Extensions.Teachers;
using MediatR;

namespace Application.Features.Teachers.Requests;

public class CreateTeacherRequestHandler(CollegeDbContext context)
    : IRequestHandler<CreateTeacherRequest, CreateTeacherResponse>
{
    public async Task<CreateTeacherResponse> Handle(CreateTeacherRequest request, CancellationToken ct)
    {
        var exists = await context.Teachers.IsNameUniqueAsync(request.Name, ct);
        if (!exists) throw new UniqueNameException(nameof(Teacher), request.Name);
        
        var teacher = new Teacher { Name = request.Name };
        
        context.Teachers.Add(teacher);
        await context.SaveChangesAsync(ct);

        return new CreateTeacherResponse(
            new TeacherDto(
                teacher.Id, teacher.Name));
    }
}