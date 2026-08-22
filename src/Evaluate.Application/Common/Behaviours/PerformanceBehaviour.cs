using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Evaluate.Application.Common.Behaviours;

public class PerformanceBehaviour<TRequest, TResponse>(ILogger<TRequest> logger) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private const int SlowRequestThresholdMilliseconds = 500;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();

        var response = await next();

        stopwatch.Stop();

        if (stopwatch.ElapsedMilliseconds > SlowRequestThresholdMilliseconds)
        {
            logger.LogWarning(
                "Long running request: {RequestName} took {ElapsedMilliseconds}ms",
                typeof(TRequest).Name,
                stopwatch.ElapsedMilliseconds);
        }

        return response;
    }
}
