using Application.Constants;
using Application.Exceptions;
using Application.Features.Teachers.Responses;
using Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Teachers.Requests;

public class UpdateTeacherRequestHandler(CollegeDbContext context) : IRequestHandler<UpdateTeacherRequest, TeacherDto>
{
    public async Task<TeacherDto> Handle(UpdateTeacherRequest request, CancellationToken cancellationToken)
    {
        var teacher =
            await context.Teachers.FirstOrDefaultAsync(s => s.Id == request.Id,
                cancellationToken: cancellationToken)
            ?? throw new NotFoundException(ValidationMessages.TeacherNotFound);

        if (!teacher.Name.Equals(request.Name, StringComparison.OrdinalIgnoreCase))
        {
            teacher.Name = request.Name;
            await context.SaveChangesAsync(cancellationToken);
        }

        return new TeacherDto(
                teacher.Id, 
                teacher.Name);
    }
}