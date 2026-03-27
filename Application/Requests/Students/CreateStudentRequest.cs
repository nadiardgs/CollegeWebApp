using Application.Responses.Students;
using MediatR;

namespace Application.Requests.Students;

public record CreateStudentRequest(string Name) : IRequest<CreateStudentResponse>;