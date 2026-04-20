using Application.Constants;

namespace Application.Exceptions;

public class TeacherAlreadyAssignedException(int courseId) : Exception(ReturnMessages.TeacherAlreadyAssigned(courseId));
