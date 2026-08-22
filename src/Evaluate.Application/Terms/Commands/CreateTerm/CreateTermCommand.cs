using Evaluate.Application.Common.Models;
using Evaluate.Application.Common.Security;
using MediatR;

namespace Evaluate.Application.Terms.Commands.CreateTerm;

[RequirePermission(Permissions.Terms.Create)]
public record CreateTermCommand(int AcademicYearId, string TermName, int TermNumber, DateOnly StartDate, DateOnly EndDate) : IRequest<Result<int>>;
