using Application.Constants;

namespace Application.Exceptions;

public class NoTeacherAssignedException(int courseId) : Exception(ReturnMessages.NoTeacherAssigned(courseId));
