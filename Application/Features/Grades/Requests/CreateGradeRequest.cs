using Application.Features.Grades.Responses;
using MediatR;

namespace Application.Features.Grades.Requests;

public record CreateGradeRequest(decimal Value, int CourseId, int StudentId) : IRequest<CreateGradeResponse>;