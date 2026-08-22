using Evaluate.Domain.Common;
using MediatR;

namespace Evaluate.Application.Common.Models;

/// <summary>Wraps a Domain-layer <see cref="BaseEvent"/> as a MediatR notification, so
/// Domain itself stays free of any dependency on MediatR.</summary>
public class DomainEventNotification<TDomainEvent>(TDomainEvent domainEvent) : INotification
    where TDomainEvent : BaseEvent
{
    public TDomainEvent DomainEvent { get; } = domainEvent;
}
