using Application.Features.Students.Responses;
using Application.Models;
using MediatR;

namespace Application.Features.Students.Requests;

public record GetAllStudentsRequest() : IRequest<ApiResult<IEnumerable<StudentDto>>>;