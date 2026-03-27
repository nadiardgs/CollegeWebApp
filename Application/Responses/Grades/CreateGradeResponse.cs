using Domain.Entities;

namespace Application.Responses.Grades;

public record CreateGradeResponse(
    int Id, 
    decimal Value, 
    string StudentName, 
    string CourseTitle
);