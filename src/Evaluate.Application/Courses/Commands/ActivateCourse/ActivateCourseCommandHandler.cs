using Evaluate.Application.Common.Exceptions;
using Evaluate.Application.Common.Interfaces;
using Evaluate.Application.Common.Models;
using MediatR;

namespace Evaluate.Application.Courses.Commands.ActivateCourse;

public class ActivateCourseCommandHandler(ICourseRepository courses, IUnitOfWork unitOfWork) : IRequestHandler<ActivateCourseCommand, Result>
{
    public async Task<Result> Handle(ActivateCourseCommand request, CancellationToken cancellationToken)
    {
        var course = await courses.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Courses.Course), request.Id);

        course.Activate();
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
