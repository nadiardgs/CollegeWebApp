using Application.Constants;

namespace Application.Exceptions;

public class MinLengthException(string name) : Exception(ErrorMessages.MinLength(name));