namespace Application.Features.Grades.Responses;

public record CreateGradeResponse(
    int Id, 
    decimal Value, 
    string StudentName, 
    string CourseTitle
);