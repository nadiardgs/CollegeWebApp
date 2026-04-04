using Application.Entities.Students.Requests;
using Application.Exceptions;
using Application.Features.Students.Responses;
using Domain.Entities;
using Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Students.Requests;

public class CreateStudentRequestHandler(CollegeDbContext context)
    : IRequestHandler<CreateStudentRequest, UpsertStudentResponse>
{
    public async Task<UpsertStudentResponse> Handle(CreateStudentRequest request, CancellationToken ct)
    {
        var exists = await context.Students.AnyAsync(s => s.Name == request.Name, ct);
        if (exists)
            throw new UniqueNameException(nameof(Student), request.Name);
        
        var student = new Student() { Name = request.Name };
        
        context.Students.Add(student);
        await context.SaveChangesAsync(ct);

        return new UpsertStudentResponse(
            new StudentDto(
                student.Id, 
                student.Name)
        );
    }


}