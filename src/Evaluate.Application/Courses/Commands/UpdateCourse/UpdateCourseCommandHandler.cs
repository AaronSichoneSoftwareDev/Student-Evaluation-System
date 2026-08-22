using Evaluate.Application.Common.Exceptions;
using Evaluate.Application.Common.Interfaces;
using Evaluate.Application.Common.Models;
using MediatR;

namespace Evaluate.Application.Courses.Commands.UpdateCourse;

public class UpdateCourseCommandHandler(ICourseRepository courses, IUnitOfWork unitOfWork) : IRequestHandler<UpdateCourseCommand, Result>
{
    public async Task<Result> Handle(UpdateCourseCommand request, CancellationToken cancellationToken)
    {
        var course = await courses.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Courses.Course), request.Id);

        course.Update(request.CourseName, request.Description);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
