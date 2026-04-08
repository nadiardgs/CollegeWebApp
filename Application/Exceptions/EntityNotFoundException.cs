using Application.Constants;

namespace Application.Exceptions;

public class EntityNotFoundException(string name, int id) : Exception(ReturnMessages.EntityNotFound(name, id));
