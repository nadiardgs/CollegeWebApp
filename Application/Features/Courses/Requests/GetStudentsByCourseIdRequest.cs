using Application.Features.Courses.Responses;
using MediatR;

namespace Application.Features.Courses.Requests;

public record GetStudentsByCourseIdRequest(int CourseId): IRequest<GetStudentsByCourseIdResponse>;