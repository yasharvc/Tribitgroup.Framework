using Test.API.Authentication.Enums;

namespace Test.API.Authentication.Contracts
{
    public interface IAuthenticationConfiguration
    {
        IEnumerable<string> ValidIPs { get; }
        IEnumerable<string> ValidAudiences { get; }
        IEnumerable<string> ValidIssuers { get; }
        bool MultiSession { get; }
        bool MultiTenant { get; }
        IDictionary<string, string> URLPermission { get; } // <URL, Permission>
        IDictionary<string, string> URLPolicy { get; }
        IDictionary<string, string> URLTenant { get; }
        IDictionary<string, DeviceTypeEnum> URLDeviceType { get; }
        IDictionary<string, string> URLSessionID { get; }
        ServerStatusEnum ServerStatus { get; }
        int MaxSessionsCount { get; }
        int MaxTokensCount { get; }
        int MaxDevicesCount { get; }
    }
}
