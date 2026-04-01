using Application.Features.Students.Responses;
using MediatR;

namespace Application.Entities.Students.Requests;

public record GetAllStudentsRequest() : IRequest<IEnumerable<StudentDto>>;