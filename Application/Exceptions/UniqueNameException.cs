using Application.Constants;

namespace Application.Exceptions;

public class UniqueNameException(string entity, string name) : Exception(ReturnMessages.UniqueName(entity, name));