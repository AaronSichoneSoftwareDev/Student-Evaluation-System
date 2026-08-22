using MediatR;

namespace Evaluate.Application.Students.Queries.GetStudentsList;

public record GetStudentsListQuery : IRequest<List<StudentDto>>;
