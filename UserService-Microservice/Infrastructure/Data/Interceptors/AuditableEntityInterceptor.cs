using Domain.Common;
using Domain.MongoEntities;
using Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using MongoDB.Driver;

namespace Infrastructure.Data.Interceptors;

public class AuditableEntityInterceptor(
    TimeProvider dateTime,
    CareerMongoDatabaseContext context,
    IUnitOfWork unitOfWork) : SaveChangesInterceptor
{
    private readonly TimeProvider _dateTime = dateTime;

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        UpdateEntities(eventData.Context).GetAwaiter().GetResult();

        return base.SavingChanges(eventData, result);
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        await UpdateEntities(eventData.Context);

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public async Task UpdateEntities(DbContext? context)
    {
        if (context == null) return;

        foreach (var entry in context.ChangeTracker.Entries<BaseAuditableEntity>())
        {
            if (entry.State is EntityState.Added or EntityState.Modified || entry.HasChangedOwnedEntities())
            {
                var utcNow = _dateTime.GetUtcNow();
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = DateTime.Now;
                }
                entry.Entity.UpdatedAt = DateTime.Now;
            }
        }        
    }
}


public static class Extensions
{
	public static bool HasChangedOwnedEntities(this EntityEntry entry) =>
		entry.References.Any(r =>
			r.TargetEntry != null &&
			r.TargetEntry.Metadata.IsOwned() &&
			(r.TargetEntry.State == EntityState.Added || r.TargetEntry.State == EntityState.Modified));
}