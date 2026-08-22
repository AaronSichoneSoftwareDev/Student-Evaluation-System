using MediatR;

namespace Evaluate.Application.Terms.Queries.GetTermsList;

public record GetTermsListQuery(int? AcademicYearId = null) : IRequest<List<TermDto>>;
