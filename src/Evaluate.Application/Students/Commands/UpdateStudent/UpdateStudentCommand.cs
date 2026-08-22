using Evaluate.Application.Common.Models;
using Evaluate.Application.Common.Security;
using Evaluate.Domain.Enums;
using MediatR;

namespace Evaluate.Application.Students.Commands.UpdateStudent;

[RequirePermission(Permissions.Students.Edit)]
public record UpdateStudentCommand(
    int Id,
    string FirstName,
    string LastName,
    DateOnly DateOfBirth,
    Gender Gender,
    string? MiddleName = null,
    string? Email = null,
    string? PhoneNumber = null,
    string? Address = null,
    int? ClassId = null) : IRequest<Result>;
