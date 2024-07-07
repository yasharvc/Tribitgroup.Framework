using StackExchange.Redis;
using Tribitgroup.Framework.Shared.Interfaces;
using Tribitgroup.Framework.Shared.Types;

namespace Tribitgroup.Framework.Shared.Services
{
    public abstract class BaseRedisCache<TValue> : ICache<string, TValue>
    {
        public TimeSpan? AutoClearTime { get; private set; } = TimeSpan.Zero;
        public ulong MaxSizeInBytes { get; private set; } = 1024 * 2;
        protected RedisSetting RedisSetting { get; private set; }
        protected IDatabase Cache { get; private set; }

        public BaseRedisCache(RedisSetting redisSetting, ulong maxSizeInBytes, TimeSpan? autoClearTime = null)
        {
            if (autoClearTime.HasValue && autoClearTime.Value < TimeSpan.Zero)
                throw new InvalidDataException(nameof(autoClearTime));

            AutoClearTime = autoClearTime ?? TimeSpan.Zero;
            RedisSetting = redisSetting;
            MaxSizeInBytes = maxSizeInBytes;
            Cache = RedisSetting.Connect();
        }

        public async Task<TValue?> GetAsync(string key)
        {
            var data = await Cache.StringGetAsync(key);
            return data.HasValue ? await StringToValueAsync(data.ToString()) : default;
        }

        public async Task<IEnumerable<TValue>> GetAllAsync()
        {
            var keyValues = new Dictionary<string, TValue>();
            var keys = await GetKeysAsync();

            foreach (var key in keys)
                keyValues[key] = await GetAsync(key) ?? throw new EntryPointNotFoundException(key);

            return keyValues.Values;
        }

        public async Task AddOrUpdateAsync(string key, TValue value, TimeSpan? expiration = null) 
            => await Cache.StringSetAsync(key, await ValueToStringAsync(value), expiration ?? AutoClearTime);

        public Task RemoveAsync(string key) => Cache.StringGetDeleteAsync(key);

        public Task<bool> ContainsKeyAsync(string key) 
            => Cache.KeyExistsAsync(key);

        public async Task ClearAsync()
        {
            var keys = await GetKeysAsync();
            foreach(var key in keys)
                await RemoveAsync(key);
        }

        protected abstract Task<TValue> StringToValueAsync(string value);
        protected abstract Task<string> ValueToStringAsync(TValue value);
        private Task<IEnumerable<string>> GetKeysAsync()
        {
            long cursor = 0;
            var keys = new List<string>();

            do
            {
                var scanResult = Cache.Execute("SCAN", cursor.ToString());
                var innerResult = (RedisResult[])scanResult;

                cursor = long.Parse((string)innerResult[0]);

                List<string> resultLines = ((string[])innerResult[1]).ToList();
                keys.AddRange(resultLines);

            } while (cursor != 0);
            return Task.FromResult(keys.AsEnumerable());
        }
    }
}
