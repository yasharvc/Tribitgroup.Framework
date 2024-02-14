namespace Tribitgroup.Framework.Shared.Interfaces.Entity
{
    public interface IHasUpdatedAt
    {
        DateTime UpdatedAt { get; }
        Task SetUpdatedAtAsync(DateTime UpdatedAt);
    }
}