namespace Tribitgroup.Framework.Shared.Interfaces.Entity
{
    public interface IHasCreatedAt
    {
        DateTime CreatedAt { get; }
        Task SetCreatedAtAsync(DateTime createdAt);
    }
}