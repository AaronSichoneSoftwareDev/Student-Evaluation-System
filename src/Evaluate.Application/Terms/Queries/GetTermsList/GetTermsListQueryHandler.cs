using Evaluate.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Evaluate.Application.Terms.Queries.GetTermsList;

public class GetTermsListQueryHandler(IApplicationDbContext context) : IRequestHandler<GetTermsListQuery, List<TermDto>>
{
    public async Task<List<TermDto>> Handle(GetTermsListQuery request, CancellationToken cancellationToken)
    {
        var query = context.Terms.AsQueryable();

        if (request.AcademicYearId.HasValue)
        {
            query = query.Where(t => t.AcademicYearId == request.AcademicYearId);
        }

        return await query
            .OrderBy(t => t.TermNumber)
            .Select(t => new TermDto(t.Id, t.AcademicYearId, t.TermName, t.TermNumber, t.StartDate, t.EndDate, t.IsActive, t.IsCurrent))
            .ToListAsync(cancellationToken);
    }
}
