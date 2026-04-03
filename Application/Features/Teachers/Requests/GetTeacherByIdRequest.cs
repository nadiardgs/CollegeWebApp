using Application.Features.Teachers.Responses;
using MediatR;

namespace Application.Features.Teachers.Requests;

public record GetTeacherByIdRequest(int Id) : IRequest<TeacherDto>;