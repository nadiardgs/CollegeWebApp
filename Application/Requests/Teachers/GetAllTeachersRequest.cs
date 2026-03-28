using Application.Responses.Teachers.DTOs;
using MediatR;

namespace Application.Requests.Teachers;

public record GetAllTeachersRequest : IRequest<IEnumerable<TeacherDto>>;