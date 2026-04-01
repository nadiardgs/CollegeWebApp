using Application.Features.Teachers.Responses;
using MediatR;

namespace Application.Features.Teachers.Requests;

public record CreateTeacherRequest(string Name) : IRequest<CreateTeacherResponse>;