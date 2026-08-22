using Evaluate.Application.Common.Interfaces;
using Evaluate.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using CourseEntity = Evaluate.Domain.Entities.Courses.Course;

namespace Evaluate.Application.Courses.Commands.CreateCourse;

public class CreateCourseCommandHandler(IApplicationDbContext context) : IRequestHandler<CreateCourseCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateCourseCommand request, CancellationToken cancellationToken)
    {
        var course = CourseEntity.Create(request.CourseCode, request.CourseName, request.Description);

        var alreadyExists = await context.Courses.AnyAsync(c => c.CourseCode == course.CourseCode, cancellationToken);
        if (alreadyExists)
        {
            return Result<int>.Failure($"A course with code '{course.CourseCode}' already exists.");
        }

        context.Courses.Add(course);
        await context.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(course.Id);
    }
}
