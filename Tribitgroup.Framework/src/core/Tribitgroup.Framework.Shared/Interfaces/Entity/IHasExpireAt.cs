namespace Tribitgroup.Framework.Shared.Interfaces.Entity
{
    public interface IHasExpireAt
    {
        DateTime ExpireAt { get; }
        Task SetExpireAtAsync(DateTime expireAt);
    }
}