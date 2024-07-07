using Test.API.Authentication.Enums;
using Test.API.Authentication.Models;

namespace Test.API.Authentication.Contracts
{
    public interface IClient<Tenant, Policy, Role, Permission>
        where Role : IRole
        where Permission : IPermission
        where Tenant : ITenant
        where Policy : IPolicy
    {
        IEnumerable<Role> Roles { get; }
        IEnumerable<Permission> Permissions { get; }
        IEnumerable<Tenant> Tenants { get; }
        IEnumerable<Policy> Policies { get; }
        string SessionId { get; }
        string ApplicationToken { get; }//Machine to machine token
        string TenantId { get; }
        string ClientToken { get; }//User login token
        string IPv4 { get; }
        string IPv6 { get; }
        int Port { get; }
        DeviceTypeEnum DeviceType { get; }
        string RequestedUrl { get; }
        string QueryString { get; }
        HttpCommandEnum HttpCommand { get; }
        string HttpScheme { get; }
        string HttpProtocol { get; }
        string Subdomain { get; }
        string Domain { get; }
        LocalizationInfo Localization { get; }
        Dictionary<Policy, PolicyEvaluatorDelegate<Tenant, Policy, Role, Permission>> PolicyEvaluators { get; }
    }
    public class HttpClient<Tenant, Policy, Role, Permission> : Client<Tenant, Policy, Role, Permission>
        where Role : IRole
        where Permission : IPermission
        where Tenant : ITenant
        where Policy : IPolicy
    {
        public HttpContext HttpContext { get; set; }
    }
}
