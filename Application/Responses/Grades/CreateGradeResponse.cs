using Domain.Entities;

namespace Application.Responses.Grades;

public record CreateGradeResponse(decimal Value, Student Student);