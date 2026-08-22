using MediatR;

namespace Evaluate.Application.Students.Queries.GetActiveStudentsList;

public class GetActiveStudentsListQueryHandler(IStudentRepository students) : IRequestHandler<GetActiveStudentsListQuery, List<ActiveStudentDto>>
{
    public Task<List<ActiveStudentDto>> Handle(GetActiveStudentsListQuery request, CancellationToken cancellationToken) =>
        students.GetActiveListAsync(cancellationToken);
}
