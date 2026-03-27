using Application.Responses.Teachers;
using MediatR;

namespace Application.Requests.Teachers;

public record CreateTeacherRequest(string Name) : IRequest<CreateTeacherResponse>;