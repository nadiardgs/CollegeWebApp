using Application.Features.Students.Responses;
using MediatR;

namespace Application.Features.Students.Requests;

public record GetStudentByIdRequest(int Id) : IRequest<GetStudentByIdResponse>;