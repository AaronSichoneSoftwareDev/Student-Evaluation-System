using System.Text.Json;
using Evaluate.Application.Common.Interfaces;
using Evaluate.Domain.Common;
using Evaluate.Domain.Entities.System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Evaluate.Infrastructure.Persistence.Interceptors;

/// <summary>Stamps Created/LastModified (At/By) on every <see cref="BaseAuditableEntity"/>
/// and writes an <see cref="AuditLog"/> row per change — so no command handler has to
/// remember to do either.</summary>
public class AuditableEntitySaveChangesInterceptor(ICurrentUserService currentUserService) : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        UpdateEntities(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        UpdateEntities(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void UpdateEntities(DbContext? context)
    {
        if (context is null)
        {
            return;
        }

        var utcNow = DateTime.UtcNow;
        var userId = currentUserService.UserId;

        foreach (var entry in context.ChangeTracker.Entries<BaseAuditableEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedBy = userId;
                entry.Entity.CreatedAt = utcNow;
            }

            if (entry.State is EntityState.Added or EntityState.Modified)
            {
                entry.Entity.LastModifiedBy = userId;
                entry.Entity.LastModifiedAt = utcNow;
            }
        }

        var auditEntries = new List<AuditLog>();

        foreach (var entry in context.ChangeTracker.Entries())
        {
            if (entry.Entity is AuditLog || entry.Entity is not BaseEntity || entry.State is EntityState.Detached or EntityState.Unchanged)
            {
                continue;
            }

            var tableName = entry.Metadata.GetTableName() ?? entry.Entity.GetType().Name;

            // Added entities don't have their generated key yet at this point in the pipeline
            // (that's assigned during this same SaveChanges call) — RecordId is left blank for
            // those rather than adding a second round-trip just to backfill it.
            string? recordId = entry.State == EntityState.Added
                ? null
                : entry.Properties.FirstOrDefault(p => p.Metadata.IsPrimaryKey())?.CurrentValue?.ToString();

            string? oldValues = null;
            string? newValues = null;

            switch (entry.State)
            {
                case EntityState.Added:
                    newValues = JsonSerializer.Serialize(entry.Properties.ToDictionary(p => p.Metadata.Name, p => p.CurrentValue));
                    break;
                case EntityState.Modified:
                    var changed = entry.Properties.Where(p => p.IsModified).ToList();
                    oldValues = JsonSerializer.Serialize(changed.ToDictionary(p => p.Metadata.Name, p => p.OriginalValue));
                    newValues = JsonSerializer.Serialize(changed.ToDictionary(p => p.Metadata.Name, p => p.CurrentValue));
                    break;
                case EntityState.Deleted:
                    oldValues = JsonSerializer.Serialize(entry.Properties.ToDictionary(p => p.Metadata.Name, p => p.OriginalValue));
                    break;
            }

            auditEntries.Add(AuditLog.Create(userId, entry.State.ToString(), tableName, recordId, oldValues, newValues));
        }

        if (auditEntries.Count > 0)
        {
            context.Set<AuditLog>().AddRange(auditEntries);
        }
    }
}
