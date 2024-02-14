using System.Text.RegularExpressions;
using Test.API.Authentication.Enums;

namespace Test.API.Authentication.Contracts
{
    public class AuthenticationConfiguration : IAuthenticationConfiguration
    {
        public IEnumerable<string> ValidIPs { get; set; } = new List<string>();
        public IEnumerable<string> ValidAudiences { get; set; } = new List<string>();
        public IEnumerable<string> ValidIssuers { get; set; } = new List<string>();
        public bool MultiSession { get; set; } = true;
        public bool MultiTenant { get; set; } = true;
        public IDictionary<string, string> URLPermission { get; set; } = new Dictionary<string, string>();
        private IDictionary<Regex, string> URLPermissionRegex => URLPermission.ToDictionary(x => new Regex(x.Key), x => x.Value);
        private Task<bool> HasUrlPermission(string url, IEnumerable<IPermission> permissions) => throw new NotImplementedException();
        public IDictionary<string, string> URLPolicy { get; set; } = new Dictionary<string, string>();

        public IDictionary<string, string> URLTenant { get; set; } = new Dictionary<string, string>();

        public IDictionary<string, DeviceTypeEnum> URLDeviceType { get; set; } = new Dictionary<string, DeviceTypeEnum>();

        public IDictionary<string, string> URLSessionID { get; set; } = new Dictionary<string, string>();

        public ServerStatusEnum ServerStatus {get; set; } = ServerStatusEnum.Available;

        public int MaxSessionsCount { get; set; } = -1;

        public int MaxTokensCount { get; set; } = -1;

        public int MaxDevicesCount { get; set; } = -1;
    }
}
