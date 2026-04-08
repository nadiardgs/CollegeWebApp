using Application.Constants;

namespace Application.Exceptions;

public class MinLengthException(string name) : Exception(ReturnMessages.MinLength(name));