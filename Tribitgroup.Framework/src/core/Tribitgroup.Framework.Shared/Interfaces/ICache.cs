using Tribitgroup.Framework.Shared.Interfaces.Entity;

namespace Tribitgroup.Framework.Shared.Interfaces
{
    public interface ICache<TKey, TValue>
    {
        Task<TValue?> GetAsync(TKey key);
        Task<IEnumerable<TValue>> GetAllAsync();
        Task AddOrUpdateAsync(TKey key, TValue value, TimeSpan? expiration = null);
        Task RemoveAsync(TKey key);
        Task<bool> ContainsKeyAsync(TKey key);
        Task ClearAsync();
        TimeSpan? AutoClearTime { get; }
        ulong MaxSizeInBytes { get; }
    }

    public interface IEntityCache<T> : ICache<Guid, T>
        where T : class, IEntity<Guid>
    { }
}