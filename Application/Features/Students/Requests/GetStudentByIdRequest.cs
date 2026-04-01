using Application.Features.Students.Responses;
using Application.Features.Students.Responses;
using MediatR;

namespace Application.Entities.Students.Requests;

public record GetStudentByIdRequest(int Id) : IRequest<GetStudentByIdResponse>;