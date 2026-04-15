namespace Application.Features.Courses.Responses;

public record StudentEnrollmentDto(int EnrollmentId, int StudentId, string StudentName, int CourseId, string CourseTitle);