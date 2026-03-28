using Application.Responses.Students.DTOs;
using MediatR;

namespace Application.Requests.Students;

public record GetAllStudentsRequest() : IRequest<IEnumerable<StudentDto>>;