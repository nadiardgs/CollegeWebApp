using Application.Constants;
using Application.Exceptions;
using Application.Features.Students.Responses;
using Domain.Entities;
using Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Students.Requests;

public class UpdateStudentRequestHandler(CollegeDbContext context) : IRequestHandler<UpdateStudentRequest, UpsertStudentResponse>
{
    public async Task<UpsertStudentResponse> Handle(UpdateStudentRequest request, CancellationToken cancellationToken)
    {
        var student =
            await context.Students.FirstOrDefaultAsync(s => s.Id == request.Id,
                cancellationToken: cancellationToken)
            ?? throw new EntityNotFoundException(nameof(Student), request.Id);

        if (!student.Name.Equals(request.Name, StringComparison.OrdinalIgnoreCase))
        {
            student.Name = request.Name;
            await context.SaveChangesAsync(cancellationToken);
        }

        return new UpsertStudentResponse(
            new StudentDto(
                student.Id, 
                student.Name)
        );
    }
}