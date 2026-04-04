using Application.Constants;

namespace Application.Exceptions;

public class EntityNotFoundException(string name, int id) : Exception(ErrorMessages.EntityNotFound(name, id));
