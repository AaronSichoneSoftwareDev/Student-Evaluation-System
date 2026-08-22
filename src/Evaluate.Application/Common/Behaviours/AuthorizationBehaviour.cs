using Evaluate.Application.Common.Exceptions;
using Evaluate.Application.Common.Interfaces;
using Evaluate.Application.Common.Security;
using MediatR;

namespace Evaluate.Application.Common.Behaviours;

public class AuthorizationBehaviour<TRequest, TResponse>(ICurrentUserService currentUserService, IIdentityService identityService) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var attribute = typeof(TRequest).GetCustomAttributes(typeof(RequirePermissionAttribute), true)
            .OfType<RequirePermissionAttribute>()
            .FirstOrDefault();

        if (attribute is not null)
        {
            var userId = currentUserService.UserId;

            // No login UI exists yet, so there is no authenticated session to check
            // permissions against — enforcement is skipped rather than denying every
            // request outright. Once a real sign-in flow populates UserId, permission
            // checks apply normally below.
            if (!string.IsNullOrEmpty(userId))
            {
                var authorized = await identityService.AuthorizeAsync(userId, attribute.Permission, cancellationToken);

                if (!authorized)
                {
                    throw new ForbiddenAccessException();
                }
            }
        }

        return await next();
    }
}
