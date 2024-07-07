using Tribitgroup.Framework.Shared.Interfaces;
using Tribitgroup.Framework.Shared.Interfaces.Entity;
using Tribitgroup.Framework.Shared.Types;

namespace Tribitgroup.Framework.Shared.Services
{
    public class RedisEntityCache<TEntity> : RedisObjectCache<TEntity>, IEntityCache<TEntity>, ICache<Guid, TEntity>
        where TEntity : class, IEntity<Guid>
    {
        public RedisEntityCache(RedisSetting redisSetting, ulong maxSizeInBytes, TimeSpan? autoClearTime = null) : base(redisSetting, maxSizeInBytes, autoClearTime)
        {
        }

        public async Task AddOrUpdateAsync(Guid key, TEntity value, TimeSpan? expiration = null) 
            => await AddOrUpdateAsync(key.ToString(), value, expiration);

        public async Task<bool> ContainsKeyAsync(Guid key)
            => await ContainsKeyAsync(key.ToString());

        public async Task<TEntity?> GetAsync(Guid key)
            => await GetAsync(key.ToString());

        public async Task RemoveAsync(Guid key)
            => await RemoveAsync(key.ToString());
    }
}