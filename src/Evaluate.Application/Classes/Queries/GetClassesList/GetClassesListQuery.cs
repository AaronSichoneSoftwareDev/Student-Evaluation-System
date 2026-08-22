using MediatR;

namespace Evaluate.Application.Classes.Queries.GetClassesList;

public record GetClassesListQuery : IRequest<List<ClassDto>>;
