using Application.Constants;

namespace Application.Exceptions;

public class CollectionNotFoundException(string name) : Exception(ReturnMessages.CollectionNotFound(name));