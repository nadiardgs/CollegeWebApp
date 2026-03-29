using Application.Responses.Students;
using MediatR;

namespace Application.Requests.Students;

public record GetStudentReportCardRequest(int StudentId) : IRequest<GetStudentReportCardResponse>;