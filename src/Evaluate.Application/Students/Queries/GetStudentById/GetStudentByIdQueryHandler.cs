using MediatR;

namespace Evaluate.Application.Students.Queries.GetStudentById;

public class GetStudentByIdQueryHandler(IStudentRepository students) : IRequestHandler<GetStudentByIdQuery, StudentDetailDto?>
{
    public Task<StudentDetailDto?> Handle(GetStudentByIdQuery request, CancellationToken cancellationToken) =>
        students.GetDetailByIdAsync(request.Id, cancellationToken);
}
