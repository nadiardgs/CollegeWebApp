using Application.Constants;
using Application.Exceptions;
using Application.Features.Teachers.Responses;
using Domain.Entities;
using Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Teachers.Requests;

public record GetTeacherByIdRequest(int Id) : IRequest<TeacherDto>;

public class GetTeacherByIdRequestHandler(CollegeDbContext context) : IRequestHandler<GetTeacherByIdRequest, TeacherDto>
{
    public async Task<TeacherDto> Handle(GetTeacherByIdRequest request, CancellationToken cancellationToken)
    {
        var result = await context.Teachers
            .AsNoTracking()
            .Where(s => s.Id == request.Id)
            .Select(s=> new TeacherDto(
                s.Id,
                s.Name))
            .FirstOrDefaultAsync(cancellationToken);
        
        return result ?? throw new EntityNotFoundException(nameof(Teacher), request.Id);
    }
}