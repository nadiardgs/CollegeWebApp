using Application.Features.Teachers.Responses;
using Application.Models;
using MediatR;

namespace Application.Features.Teachers.Requests;

public record GetAllTeachersRequest : IRequest<ApiResult<IEnumerable<TeacherDto>>>;