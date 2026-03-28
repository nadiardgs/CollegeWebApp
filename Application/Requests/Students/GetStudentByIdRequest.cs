using Application.Responses.Students;
using MediatR;

namespace Application.Requests.Students;

public record GetStudentByIdRequest(int Id) : IRequest<GetStudentByIdResponse>;