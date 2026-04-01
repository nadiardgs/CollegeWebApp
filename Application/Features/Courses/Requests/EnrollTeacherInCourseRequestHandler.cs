using Application.Constants;
using Application.Exceptions;
using Application.Features.Courses.Responses;
using Infrastructure;
using MediatR;

namespace Application.Features.Courses.Requests;

public class EnrollTeacherInCourseRequestHandler(CollegeDbContext context) : IRequestHandler<EnrollTeacherInCourseRequest, EnrollTeacherInCourseResponse>
{
    public async Task<EnrollTeacherInCourseResponse> Handle(EnrollTeacherInCourseRequest request, CancellationToken cancellationToken)
    {
        var course = await context.Courses.FindAsync([request.CourseId], cancellationToken)
                     ?? throw new NotFoundException(ValidationMessages.CourseNotFound);

        if (course.TeacherId == request.TeacherId) 
            return new EnrollTeacherInCourseResponse(true);

        course.TeacherId = request.TeacherId;

        await context.SaveChangesAsync(cancellationToken);
        
        return new EnrollTeacherInCourseResponse(true);
    }
}