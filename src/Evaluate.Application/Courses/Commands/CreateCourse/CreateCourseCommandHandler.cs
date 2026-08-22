using Evaluate.Application.Common.Interfaces;
using Evaluate.Application.Common.Models;
using CourseEntity = Evaluate.Domain.Entities.Courses.Course;
using MediatR;

namespace Evaluate.Application.Courses.Commands.CreateCourse;

public class CreateCourseCommandHandler(ICourseRepository courses, IUnitOfWork unitOfWork) : IRequestHandler<CreateCourseCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateCourseCommand request, CancellationToken cancellationToken)
    {
        var course = CourseEntity.Create(request.CourseCode, request.CourseName, request.Description);

        var alreadyExists = await courses.ExistsByCodeAsync(course.CourseCode, cancellationToken);
        if (alreadyExists)
        {
            return Result<int>.Failure($"A course with code '{course.CourseCode}' already exists.");
        }

        courses.Add(course);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(course.Id);
    }
}
