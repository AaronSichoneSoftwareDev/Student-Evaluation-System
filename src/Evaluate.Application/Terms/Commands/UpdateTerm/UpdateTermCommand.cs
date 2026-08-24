using Evaluate.Application.Common.Models;
using Evaluate.Application.Common.Security;
using MediatR;

namespace Evaluate.Application.Terms.Commands.UpdateTerm;

[RequirePermission(Permissions.Terms.Edit)]
public record UpdateTermCommand(int Id, string TermName, int TermNumber, DateOnly StartDate, DateOnly EndDate) : IRequest<Result>;
