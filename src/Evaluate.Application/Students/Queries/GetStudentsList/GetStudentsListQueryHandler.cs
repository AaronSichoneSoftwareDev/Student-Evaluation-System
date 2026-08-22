using MediatR;

namespace Evaluate.Application.Students.Queries.GetStudentsList;

public class GetStudentsListQueryHandler(IStudentRepository students) : IRequestHandler<GetStudentsListQuery, List<StudentDto>>
{
    public Task<List<StudentDto>> Handle(GetStudentsListQuery request, CancellationToken cancellationToken) =>
        students.GetListAsync(cancellationToken);
}
