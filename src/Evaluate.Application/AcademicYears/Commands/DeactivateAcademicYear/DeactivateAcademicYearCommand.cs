using Evaluate.Application.Common.Models;
using Evaluate.Application.Common.Security;
using MediatR;

namespace Evaluate.Application.AcademicYears.Commands.DeactivateAcademicYear;

[RequirePermission(Permissions.AcademicYears.Edit)]
public record DeactivateAcademicYearCommand(int Id) : IRequest<Result>;
