using Application.Constants;

namespace Application.Exceptions;

public class StudentAlreadyEnrolledException(int studentId, int courseId) : Exception(ReturnMessages.AlreadyEnrolled(studentId, courseId));