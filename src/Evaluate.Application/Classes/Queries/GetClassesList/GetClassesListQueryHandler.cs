using MediatR;

namespace Evaluate.Application.Classes.Queries.GetClassesList;

public class GetClassesListQueryHandler(IClassRepository classes) : IRequestHandler<GetClassesListQuery, List<ClassDto>>
{
    public Task<List<ClassDto>> Handle(GetClassesListQuery request, CancellationToken cancellationToken) =>
        classes.GetListAsync(cancellationToken);
}
