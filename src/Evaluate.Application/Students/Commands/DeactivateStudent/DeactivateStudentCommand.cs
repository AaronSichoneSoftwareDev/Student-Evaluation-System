using Evaluate.Application.Common.Models;
using Evaluate.Application.Common.Security;
using MediatR;

namespace Evaluate.Application.Students.Commands.DeactivateStudent;

[RequirePermission(Permissions.Students.Edit)]
public record DeactivateStudentCommand(int Id) : IRequest<Result>;
