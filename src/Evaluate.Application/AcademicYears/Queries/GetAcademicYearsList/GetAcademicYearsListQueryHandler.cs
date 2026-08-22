using Evaluate.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Evaluate.Application.AcademicYears.Queries.GetAcademicYearsList;

public class GetAcademicYearsListQueryHandler(IApplicationDbContext context) : IRequestHandler<GetAcademicYearsListQuery, List<AcademicYearDto>>
{
    public async Task<List<AcademicYearDto>> Handle(GetAcademicYearsListQuery request, CancellationToken cancellationToken) =>
        await context.AcademicYears
            .OrderByDescending(y => y.StartDate)
            .Select(y => new AcademicYearDto(y.Id, y.YearName, y.StartDate, y.EndDate, y.IsCurrent, y.IsActive))
            .ToListAsync(cancellationToken);
}
