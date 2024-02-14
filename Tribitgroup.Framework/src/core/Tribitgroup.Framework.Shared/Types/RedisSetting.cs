using StackExchange.Redis;

namespace Tribitgroup.Framework.Shared.Types
{
    public class RedisSetting
    {
        public string ConnectionString { get; set; } = string.Empty;

        internal IDatabase Connect()
        {
            if(ConnectionString == string.Empty)
                throw new NotImplementedException();
            return ConnectionMultiplexer.Connect(ConnectionString).GetDatabase();
        }
    }
}