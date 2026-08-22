using Evaluate.Application.Common.Interfaces;
using Evaluate.Application.Common.Models;
using AcademicYearEntity = Evaluate.Domain.Entities.Academic.AcademicYear;
using MediatR;

namespace Evaluate.Application.AcademicYears.Commands.CreateAcademicYear;

public class CreateAcademicYearCommandHandler(IAcademicYearRepository academicYears, IUnitOfWork unitOfWork) : IRequestHandler<CreateAcademicYearCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateAcademicYearCommand request, CancellationToken cancellationToken)
    {
        var alreadyExists = await academicYears.ExistsByNameAsync(request.YearName, cancellationToken);
        if (alreadyExists)
        {
            return Result<int>.Failure($"An academic year named '{request.YearName}' already exists.");
        }

        var academicYear = AcademicYearEntity.Create(request.YearName, request.StartDate, request.EndDate);

        academicYears.Add(academicYear);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(academicYear.Id);
    }
}
