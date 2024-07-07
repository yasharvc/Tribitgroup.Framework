using Tribitgroup.Framework.Shared.Interfaces.Entity;

namespace Tribitgroup.Framework.Shared.Types
{
    public abstract class AuditedAggregateRoot<T> : Entity<T>, IHasCreatedAt, IHasCreatedBy, IAggregateRoot<T> where T : notnull
    {
        public Guid CreatedBy { get; protected set; }
        public Guid? UpdatedBy { get; protected set; }
        public DateTime? UpdatedOn { get; protected set; }
        public DateTime CreatedAt { get; protected set; }

        public AuditedAggregateRoot()
        {
            CreatedAt = DateTime.UtcNow;
        }

        public virtual Task SetCreatedByAsync(Guid userId)
        {
            CreatedBy = userId;
            return Task.CompletedTask;
        }

        public abstract Task SetCreatedAtAsync(DateTime createdAt);
    }
    public abstract class AuditedAggregateRoot : AuditedAggregateRoot<Guid>, IAggregateRoot
    {
    }
}