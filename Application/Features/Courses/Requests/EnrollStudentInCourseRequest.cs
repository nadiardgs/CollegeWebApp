using Application.Features.Courses.Responses;
using MediatR;

namespace Application.Features.Courses.Requests;

public record EnrollStudentInCourseRequest (int CourseId, int StudentId) : IRequest<EnrollStudentInCourseResponse>;