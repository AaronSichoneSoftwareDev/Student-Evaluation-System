using Evaluate.Application.Common.Models;
using Evaluate.Application.Common.Security;
using MediatR;

namespace Evaluate.Application.Terms.Commands.DeactivateTerm;

[RequirePermission(Permissions.Terms.Edit)]
public record DeactivateTermCommand(int Id) : IRequest<Result>;
