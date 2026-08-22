using Evaluate.Application.Common.Models;
using Evaluate.Application.Common.Security;
using MediatR;

namespace Evaluate.Application.Identity.Commands.RegisterUser;

[RequirePermission(Permissions.Users.Create)]
public record RegisterUserCommand(string UserName, string Email, string Password, string FirstName, string LastName, string Role) : IRequest<Result<string>>;
