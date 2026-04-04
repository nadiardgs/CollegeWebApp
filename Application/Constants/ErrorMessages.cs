namespace Application.Constants;

public static class ErrorMessages
{
    public static string EntityNotFound(string name, int id) 
        => $"{name} with ID {id} was not found.";

    public static string CollectionNotFound(string name) 
        => $"No entity of type {name}s found.";

    public static string MinLength(string name)
        => $"{name} name must have at least 3 letters.";
    
    public static string UniqueName(string entity, string name)
    => $"A ${entity} with name {name} name already exists.";
}