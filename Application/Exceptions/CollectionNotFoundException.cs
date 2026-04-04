using Application.Constants;

namespace Application.Exceptions;

public class CollectionNotFoundException(string name) : Exception(ErrorMessages.CollectionNotFound(name));