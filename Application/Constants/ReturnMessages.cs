namespace Application.Constants;

public static class ReturnMessages
{
    public static string EntityNotFound(string name, int id) 
        => $"{name} with ID {id} was not found.";

    public static string CollectionNotFound(string name) 
        => $"No objects of type {name} found.";

    public static string MinLength(string name)
        => $"{name}'s name must have at least 3 letters.";
    
    public static string UniqueName(string entity, string name)
        => $"A ${entity} with name {name} already exists.";
    
    public static string Success(int count, string entity)
        => $"{count} objects of type {entity} found.";
    
    public static string AlreadyEnrolled(int studentId, int courseId)
        => $"Student {studentId} has already been enrolled in course {courseId}.";
    
    public static string NotEnrolled(int studentId, int courseId)
        => $"Student {studentId} is not enrolled in course {courseId} and therefore can't be graded.";
    
    public static string GradeOutOfRange()
        => "Grades must be between 0 and 10.";
}