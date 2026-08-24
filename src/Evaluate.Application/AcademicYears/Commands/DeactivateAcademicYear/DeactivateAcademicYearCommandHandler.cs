using Evaluate.Application.Common.Exceptions;
using Evaluate.Application.Common.Interfaces;
using Evaluate.Application.Common.Models;
using MediatR;

namespace Evaluate.Application.AcademicYears.Commands.DeactivateAcademicYear;

public class DeactivateAcademicYearCommandHandler(IAcademicYearRepository academicYears, IUnitOfWork unitOfWork) : IRequestHandler<DeactivateAcademicYearCommand, Result>
{
    public async Task<Result> Handle(DeactivateAcademicYearCommand request, CancellationToken cancellationToken)
    {
        var academicYear = await academicYears.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Academic.AcademicYear), request.Id);

        if (academicYear.IsCurrent)
        {
            return Result.Failure("This is the current academic year — mark a different year as current before deactivating it.");
        }

        academicYear.Deactivate();
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
