using Evaluate.Application.Common.Exceptions;
using Evaluate.Application.Common.Interfaces;
using Evaluate.Application.Common.Models;
using MediatR;

namespace Evaluate.Application.Courses.Commands.DeactivateCourse;

public class DeactivateCourseCommandHandler(ICourseRepository courses, IUnitOfWork unitOfWork) : IRequestHandler<DeactivateCourseCommand, Result>
{
    public async Task<Result> Handle(DeactivateCourseCommand request, CancellationToken cancellationToken)
    {
        var course = await courses.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Courses.Course), request.Id);

        course.Deactivate();
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
