using Application.Exceptions;
using Application.Features.Teachers.Responses;
using Domain.Entities;
using Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Teachers.Requests;

public class GetAllTeachersRequestHandler(CollegeDbContext context) : IRequestHandler<GetAllTeachersRequest, IEnumerable<TeacherDto>>
{
    public async Task<IEnumerable<TeacherDto>> Handle(GetAllTeachersRequest request, CancellationToken cancellationToken)
    {
        var result = await context.Teachers
            .AsNoTracking()
            .Select(s => new TeacherDto(s.Id, s.Name))
            .ToListAsync(cancellationToken: cancellationToken);
        
        return result ?? throw new CollectionNotFoundException(nameof(Teacher));
    }
}