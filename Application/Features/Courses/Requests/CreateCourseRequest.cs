using Application.Features.Courses.Responses;
using MediatR;

namespace Application.Features.Courses.Requests;

public record CreateCourseRequest(string Title, int TeacherId) : IRequest<CreateCourseResponse>;