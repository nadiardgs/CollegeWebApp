using Application.Constants;

namespace Application.Exceptions;

public class UniqueNameException(string entity, string name) : Exception(ErrorMessages.UniqueName(entity, name));