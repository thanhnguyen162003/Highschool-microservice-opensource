using Domain.CustomEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Infrastructure.Persistence
{
    public class AuditableEntityInterceptor(TimeProvider timeProvider) : SaveChangesInterceptor
    {
        private readonly TimeProvider _timeProvider = timeProvider;

        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            UpdateEntities(eventData.Context).GetAwaiter();

            return base.SavingChanges(eventData, result);
        }

        public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            await UpdateEntities(eventData.Context);

            return await base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        public async Task UpdateEntities(DbContext? context)
        {
            if (context == null)
            {
                return;
            }

            foreach (var entry in context.ChangeTracker.Entries<BaseAuditableEntity>())
            {
                if (entry.State is EntityState.Added or EntityState.Modified || entry.HasChangedOwnedEntities())
                {
                    var utcNow = _timeProvider.GetUtcNow();
                    if (entry.State == EntityState.Added)
                    {
                        entry.Entity.CreatedAt = DateTime.UtcNow;
                    }
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                }
            }

            await Task.CompletedTask;

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
}
