using Application.Responses.Courses.DTOs;
using MediatR;

namespace Application.Requests.Courses;

public record GetAllCoursesRequest : IRequest<IEnumerable<CourseDto>>;