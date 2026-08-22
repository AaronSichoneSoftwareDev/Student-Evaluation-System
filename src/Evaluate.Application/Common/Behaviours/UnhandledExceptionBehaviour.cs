using MediatR;
using Microsoft.Extensions.Logging;

namespace Evaluate.Application.Common.Behaviours;

public class UnhandledExceptionBehaviour<TRequest, TResponse>(ILogger<TRequest> logger) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        try
        {
            return await next();
        }
        catch (Exception ex) when (ex is not Exceptions.ValidationException and not Exceptions.ForbiddenAccessException and not Exceptions.NotFoundException)
        {
            logger.LogError(ex, "Unhandled exception for request {RequestName}", typeof(TRequest).Name);
            throw;
        }
    }
}
