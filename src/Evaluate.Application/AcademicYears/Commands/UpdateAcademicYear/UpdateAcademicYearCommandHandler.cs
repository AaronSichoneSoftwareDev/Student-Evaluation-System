using Evaluate.Application.Common.Exceptions;
using Evaluate.Application.Common.Interfaces;
using Evaluate.Application.Common.Models;
using MediatR;

namespace Evaluate.Application.AcademicYears.Commands.UpdateAcademicYear;

public class UpdateAcademicYearCommandHandler(IAcademicYearRepository academicYears, IUnitOfWork unitOfWork) : IRequestHandler<UpdateAcademicYearCommand, Result>
{
    public async Task<Result> Handle(UpdateAcademicYearCommand request, CancellationToken cancellationToken)
    {
        var academicYear = await academicYears.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Academic.AcademicYear), request.Id);

        var duplicateName = await academicYears.ExistsByNameAsync(request.YearName, request.Id, cancellationToken);
        if (duplicateName)
        {
            return Result.Failure($"An academic year named '{request.YearName}' already exists.");
        }

        academicYear.Update(request.YearName, request.StartDate, request.EndDate);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
