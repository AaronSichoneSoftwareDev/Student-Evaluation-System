using Evaluate.Application.Common.Interfaces;
using Evaluate.Application.Common.Models;
using AcademicYearEntity = Evaluate.Domain.Entities.Academic.AcademicYear;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Evaluate.Application.AcademicYears.Commands.CreateAcademicYear;

public class CreateAcademicYearCommandHandler(IApplicationDbContext context) : IRequestHandler<CreateAcademicYearCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateAcademicYearCommand request, CancellationToken cancellationToken)
    {
        var alreadyExists = await context.AcademicYears.AnyAsync(y => y.YearName == request.YearName, cancellationToken);
        if (alreadyExists)
        {
            return Result<int>.Failure($"An academic year named '{request.YearName}' already exists.");
        }

        var academicYear = AcademicYearEntity.Create(request.YearName, request.StartDate, request.EndDate);

        context.AcademicYears.Add(academicYear);
        await context.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(academicYear.Id);
    }
}
