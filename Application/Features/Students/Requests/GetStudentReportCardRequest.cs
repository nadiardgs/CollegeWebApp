using Application.Features.Students.Responses;
using MediatR;

namespace Application.Features.Students.Requests;

public record GetStudentReportCardRequest(int StudentId) : IRequest<GetStudentReportCardResponse>;