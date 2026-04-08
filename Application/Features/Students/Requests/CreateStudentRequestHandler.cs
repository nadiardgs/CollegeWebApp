using Application.Exceptions;
using Application.Features.Students.Responses;
using Domain.Entities;
using Infrastructure;
using MediatR;
using Infrastructure.Extensions.Students;

namespace Application.Features.Students.Requests;

public record CreateStudentRequest(string Name) : IRequest<StudentDto>;

public class CreateStudentRequestHandler(CollegeDbContext context)
    : IRequestHandler<CreateStudentRequest, StudentDto>
{
    public async Task<StudentDto> Handle(CreateStudentRequest request, CancellationToken ct)
    {
        var exists = await context.Students.IsNameUniqueAsync(request.Name, ct);
            if (!exists) throw new UniqueNameException(nameof(Student), request.Name);
        
        var student = new Student() { Name = request.Name };
        
        context.Students.Add(student);
        await context.SaveChangesAsync(ct);

        return new StudentDto(
                student.Id, 
                student.Name);
    }


}