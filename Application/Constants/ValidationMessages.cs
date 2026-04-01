namespace Application.Constants;

public static class ValidationMessages
{
    public const string StudentNameMinLength = "Student name must have at least 3 letters.";
    public const string TeacherNameMinLength = "Teacher name must have at least 3 letters.";
    public const string TeacherUniqueName = "A teacher with this name already exists.";
    public const string GradeRange = "Grade must be between 0 and 20.";
    public const string StudentAlreadyExists = "A student with this name already exists.";
    public const string ValidationFailed = "Validation Failed";
    public const string StudentNotFound = "Student does not exist.";
    public const string BookNotFound = "Book does not exist.";
    public const string CourseNotFound = "Course does not exist.";
}