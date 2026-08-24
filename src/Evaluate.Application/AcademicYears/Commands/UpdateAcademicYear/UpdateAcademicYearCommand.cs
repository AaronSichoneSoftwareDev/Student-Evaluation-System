using Evaluate.Application.Common.Models;
using Evaluate.Application.Common.Security;
using MediatR;

namespace Evaluate.Application.AcademicYears.Commands.UpdateAcademicYear;

[RequirePermission(Permissions.AcademicYears.Edit)]
public record UpdateAcademicYearCommand(int Id, string YearName, DateOnly StartDate, DateOnly EndDate) : IRequest<Result>;
