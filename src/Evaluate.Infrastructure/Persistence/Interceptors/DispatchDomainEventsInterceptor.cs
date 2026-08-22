using Evaluate.Application.Common.Models;
using Evaluate.Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Evaluate.Infrastructure.Persistence.Interceptors;

/// <summary>Publishes each aggregate's recorded domain events, via MediatR, once
/// <c>SaveChanges</c> has actually succeeded — so a handler never reacts to an event
/// whose change didn't make it to the database.</summary>
public class DispatchDomainEventsInterceptor(IPublisher publisher) : SaveChangesInterceptor
{
    public override async ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result, CancellationToken cancellationToken = default)
    {
        await DispatchDomainEvents(eventData.Context);
        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }

    private async Task DispatchDomainEvents(DbContext? context)
    {
        if (context is null)
        {
            return;
        }

        var entitiesWithEvents = context.ChangeTracker.Entries<BaseAuditableEntity>()
            .Select(e => e.Entity)
            .Where(e => e.DomainEvents.Count > 0)
            .ToList();

        var domainEvents = entitiesWithEvents.SelectMany(e => e.DomainEvents).ToList();

        foreach (var entity in entitiesWithEvents)
        {
            entity.ClearDomainEvents();
        }

        foreach (var domainEvent in domainEvents)
        {
            var wrapperType = typeof(DomainEventNotification<>).MakeGenericType(domainEvent.GetType());
            var notification = (INotification)Activator.CreateInstance(wrapperType, domainEvent)!;
            await publisher.Publish(notification);
        }
    }
}
