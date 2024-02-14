namespace Tribitgroup.Framework.Shared.Interfaces.Entity
{
    public interface ICachableEntity
    {
        string GetCacheKey() => GetType().FullName ?? "";
        TimeSpan? GetExpireTime() => new(TimeSpan.TicksPerDay * 365);
    }
}
