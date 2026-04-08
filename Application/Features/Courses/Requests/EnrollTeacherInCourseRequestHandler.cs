using Application.Exceptions;
using Application.Features.Courses.Responses;
using Domain.Entities;
using Infrastructure;
using MediatR;

namespace Application.Features.Courses.Requests;

public record EnrollTeacherInCourseRequest(int CourseId, int TeacherId) : IRequest<EnrollTeacherInCourseResponse>;

public class EnrollTeacherInCourseRequestHandler(CollegeDbContext context) : IRequestHandler<EnrollTeacherInCourseRequest, EnrollTeacherInCourseResponse>
{
    public async Task<EnrollTeacherInCourseResponse> Handle(EnrollTeacherInCourseRequest request, CancellationToken cancellationToken)
    {
        var course = await context.Courses.FindAsync([request.CourseId], cancellationToken)
                     ?? throw new EntityNotFoundException(nameof(Course), request.CourseId);

        if (course.TeacherId == request.TeacherId) 
            return new EnrollTeacherInCourseResponse(true);

        course.TeacherId = request.TeacherId;

        await context.SaveChangesAsync(cancellationToken);
        
        return new EnrollTeacherInCourseResponse(true);
    }
}