using Application.Constants;
using Application.Features.Courses.Requests;
using Domain.Entities;
using FluentValidation;
using Infrastructure;
using Infrastructure.Extensions.Enrollments;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Courses.Validators;

public class CourseRequestValidator : AbstractValidator<EnrollStudentInCourseRequest>
{
    public CourseRequestValidator(CollegeDbContext context)
    {
        RuleFor(request => request.CourseId)
            .MustAsync(async (id, ct) 
                => await context.Courses.AnyAsync(c => c.Id == id, ct))
            .WithMessage(request => ReturnMessages.EntityNotFound(nameof(Course), request.CourseId));
        
        RuleFor(request => request.StudentId)
            .MustAsync(async (id, ct) 
                => await context.Students.AnyAsync(s => s.Id == id, ct))
            .WithMessage(request => ReturnMessages.EntityNotFound(nameof(Student), request.StudentId));
        
        RuleFor(x => x)
            .MustAsync(async (request, ct) => 
                !await context.Enrollments.IsEnrolledAsync(request.StudentId, request.CourseId, ct)) 
            .WithMessage(request => ReturnMessages.AlreadyEnrolled(request.StudentId, request.CourseId));
    }
}