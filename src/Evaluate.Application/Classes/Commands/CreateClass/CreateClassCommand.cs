using Evaluate.Application.Common.Models;
using Evaluate.Application.Common.Security;
using MediatR;

namespace Evaluate.Application.Classes.Commands.CreateClass;

[RequirePermission(Permissions.Classes.Create)]
public record CreateClassCommand(string ClassName, string GradeLevel, string? Description = null) : IRequest<Result<int>>;
