using MediatR;

namespace Evaluate.Application.AcademicYears.Queries.GetAcademicYearsList;

public record GetAcademicYearsListQuery : IRequest<List<AcademicYearDto>>;
