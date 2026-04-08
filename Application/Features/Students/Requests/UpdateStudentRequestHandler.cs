using Application.Exceptions;
using Application.Features.Students.Responses;
using Domain.Entities;
using Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Students.Requests;

public class UpdateStudentRequestHandler(CollegeDbContext context) : IRequestHandler<UpdateStudentRequest, StudentDto>
{
    public async Task<StudentDto> Handle(UpdateStudentRequest request, CancellationToken cancellationToken)
    {
        var student =
            await context.Students.FirstOrDefaultAsync(s => s.Id == request.Id,
                cancellationToken: cancellationToken)
            ?? throw new EntityNotFoundException(nameof(Student), request.Id);

        var studentDto = new StudentDto(
                student.Id,
                student.Name);
        
        if (student.Name.Equals(request.Name, StringComparison.OrdinalIgnoreCase))
            return studentDto;
        
        student.Name = request.Name;
        await context.SaveChangesAsync(cancellationToken);

        return studentDto;
    }
}