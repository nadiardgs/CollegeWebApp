using Application.Responses.Courses;
using MediatR;

namespace Application.Requests.Courses;

public record CreateCourseRequest(string Title, int TeacherId) : IRequest<CreateCourseResponse>;