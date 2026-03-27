using Application.Responses.Courses;
using MediatR;

namespace Application.Requests.Courses;

public record GetStudentsByCourseIdRequest(int CourseId): IRequest<GetStudentsByCourseIdResponse>;