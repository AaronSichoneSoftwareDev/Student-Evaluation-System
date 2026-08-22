using MediatR;

namespace Evaluate.Application.AcademicYears.Queries.GetAcademicYearsList;

public class GetAcademicYearsListQueryHandler(IAcademicYearRepository academicYears) : IRequestHandler<GetAcademicYearsListQuery, List<AcademicYearDto>>
{
    public Task<List<AcademicYearDto>> Handle(GetAcademicYearsListQuery request, CancellationToken cancellationToken) =>
        academicYears.GetListAsync(cancellationToken);
}
