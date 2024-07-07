using Tribitgroup.Framework.Shared.Extensions;
using Tribitgroup.Framework.Shared.Interfaces;
using Tribitgroup.Framework.Shared.Types;

namespace Tribitgroup.Framework.Shared.Services
{
    public class RedisObjectCache<TValue> : BaseRedisCache<TValue>, ICache<string, TValue> where TValue : class
    {
        public RedisObjectCache(RedisSetting redisSetting, ulong maxSizeInBytes, TimeSpan? autoClearTime = null) 
            : base(redisSetting, maxSizeInBytes, autoClearTime)
        {
            if(typeof(TValue).IsBasicType())
                throw new ArgumentException($"{typeof(TValue).FullName} should not be a basic type.");
        }

        protected override Task<TValue> StringToValueAsync(string value) 
            => Task.FromResult(value.FromJson<TValue>() ?? throw new InvalidCastException($"`{value}` into `{typeof(TValue).FullName}` type"));

        protected override Task<string> ValueToStringAsync(TValue value)
            => Task.FromResult(value.ToJson());
    }
}