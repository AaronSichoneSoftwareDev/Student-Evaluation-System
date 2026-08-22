using Evaluate.Application.Common.Models;
using Evaluate.Application.Common.Security;
using MediatR;

namespace Evaluate.Application.AcademicYears.Commands.CreateAcademicYear;

[RequirePermission(Permissions.AcademicYears.Create)]
public record CreateAcademicYearCommand(string YearName, DateOnly StartDate, DateOnly EndDate) : IRequest<Result<int>>;
