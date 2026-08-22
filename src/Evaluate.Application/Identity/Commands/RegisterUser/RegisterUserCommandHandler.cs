using Evaluate.Application.Common.Interfaces;
using Evaluate.Application.Common.Models;
using MediatR;

namespace Evaluate.Application.Identity.Commands.RegisterUser;

public class RegisterUserCommandHandler(IIdentityService identityService) : IRequestHandler<RegisterUserCommand, Result<string>>
{
    public async Task<Result<string>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var (result, userId) = await identityService.CreateUserAsync(
            request.UserName,
            request.Email,
            request.Password,
            request.FirstName,
            request.LastName,
            cancellationToken);

        if (!result.Succeeded)
        {
            return Result<string>.Failure(result.Errors);
        }

        var roleResult = await identityService.AddToRoleAsync(userId, request.Role, cancellationToken);
        if (!roleResult.Succeeded)
        {
            return Result<string>.Failure(roleResult.Errors);
        }

        return Result<string>.Success(userId);
    }
}
