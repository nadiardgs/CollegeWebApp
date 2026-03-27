using Application.Responses.Grades;
using Domain.Entities;
using MediatR;

namespace Application.Requests.Grades;

public record CreateGradeRequest(decimal Value, int CourseId, int StudentId) : IRequest<CreateGradeResponse>;