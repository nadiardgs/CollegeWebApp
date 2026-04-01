using Application.Features.Teachers.Responses;
using MediatR;

namespace Application.Features.Teachers.Requests;

public record GetAllTeachersRequest : IRequest<IEnumerable<TeacherDto>>;