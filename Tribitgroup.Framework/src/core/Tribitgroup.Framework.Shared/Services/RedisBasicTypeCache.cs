using Tribitgroup.Framework.Shared.Extensions;
using Tribitgroup.Framework.Shared.Interfaces;
using Tribitgroup.Framework.Shared.Types;

namespace Tribitgroup.Framework.Shared.Services
{
    public class RedisBasicTypeCache<TValue> : BaseRedisCache<TValue>, ICache<string, TValue>
    {

        public RedisBasicTypeCache(RedisSetting redisSetting, ulong maxSizeInBytes, TimeSpan? autoClearTime = null)
            : base(redisSetting, maxSizeInBytes, autoClearTime)
        {
            if (!typeof(TValue).IsBasicType())
                throw new ArgumentException($"{typeof(TValue).FullName} is not basic type.");
        }

        protected override Task<TValue> StringToValueAsync(string value) 
            => Task.FromResult(value.To<TValue>() ?? throw new InvalidCastException($"`{value}` into `{typeof(TValue).FullName}` type"));

        protected override Task<string> ValueToStringAsync(TValue value)
        {
            return Task.FromResult(value?.ToString() ?? throw new InvalidCastException($"`{value}` into `string` type"));
        }
    }
}
