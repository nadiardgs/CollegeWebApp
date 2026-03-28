using Application.Exceptions;
using Application.Responses.Teachers.DTOs;
using Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Requests.Teachers;

public class GetAllTeachersRequestHandler(CollegeDbContext context) : IRequestHandler<GetAllTeachersRequest, IEnumerable<TeacherDto>>
{
    public async Task<IEnumerable<TeacherDto>> Handle(GetAllTeachersRequest request, CancellationToken cancellationToken)
    {
        var result = await context.Teachers
            .AsNoTracking()
            .Select(s => new TeacherDto(s.Id, s.Name))
            .ToListAsync(cancellationToken: cancellationToken);
        
        return result ?? throw new NotFoundException($"No teacher was found.");
    }
}