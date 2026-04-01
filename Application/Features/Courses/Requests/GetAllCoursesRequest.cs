using Application.Features.Courses.Responses;
using MediatR;

namespace Application.Features.Courses.Requests;

public record GetAllCoursesRequest : IRequest<IEnumerable<CourseDto>>;