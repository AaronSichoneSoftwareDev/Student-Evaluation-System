using MediatR;

namespace Evaluate.Application.Students.Queries.GetStudentById;

public record GetStudentByIdQuery(int Id) : IRequest<StudentDetailDto?>;
