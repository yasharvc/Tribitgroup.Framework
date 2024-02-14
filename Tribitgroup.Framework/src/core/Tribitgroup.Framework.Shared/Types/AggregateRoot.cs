using Tribitgroup.Framework.Shared.Interfaces.Entity;

namespace Tribitgroup.Framework.Shared.Types
{
    public abstract class AggregateRoot<T> : Entity<T>, IAggregateRoot<T> where T : notnull
    {
    }
    public abstract class AggregateRoot : Entity, IAggregateRoot
    {
    }
}