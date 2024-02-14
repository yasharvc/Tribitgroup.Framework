namespace Tribitgroup.Framework.Shared.Interfaces.Entity
{
    public interface IAggregateRoot<T> : IEntity<T> where T : notnull
    {
    }
    public interface IAggregateRoot : IAggregateRoot<Guid>
    {
    }
}