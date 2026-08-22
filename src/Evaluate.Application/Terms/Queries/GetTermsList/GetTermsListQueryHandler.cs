using MediatR;

namespace Evaluate.Application.Terms.Queries.GetTermsList;

public class GetTermsListQueryHandler(ITermRepository terms) : IRequestHandler<GetTermsListQuery, List<TermDto>>
{
    public Task<List<TermDto>> Handle(GetTermsListQuery request, CancellationToken cancellationToken) =>
        terms.GetListAsync(request.AcademicYearId, cancellationToken);
}
