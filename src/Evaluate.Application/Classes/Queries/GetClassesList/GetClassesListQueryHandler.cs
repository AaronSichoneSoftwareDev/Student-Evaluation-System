using Evaluate.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Evaluate.Application.Classes.Queries.GetClassesList;

public class GetClassesListQueryHandler(IApplicationDbContext context) : IRequestHandler<GetClassesListQuery, List<ClassDto>>
{
    public async Task<List<ClassDto>> Handle(GetClassesListQuery request, CancellationToken cancellationToken) =>
        await context.Classes
            .OrderBy(c => c.ClassName)
            .Select(c => new ClassDto(c.Id, c.ClassName, c.GradeLevel, c.Description, c.IsActive))
            .ToListAsync(cancellationToken);
}
