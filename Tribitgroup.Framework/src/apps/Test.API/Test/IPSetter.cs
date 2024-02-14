using System.Net;
using Test.API.Authentication.Contracts;

namespace Test.API.Test
{
    public class IPSetter : HttpPreauthenticatePipeStep<Tenant, Policy, Role, Permission>
    {
        public override Task<HttpClient<Tenant, Policy, Role, Permission>> ExecuteAsync(HttpClient<Tenant, Policy, Role, Permission> client, IAuthenticationConfiguration configuration)
        {
            client.IPv4 = client.HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString() ?? "";
            if (client.IPv4 == "0.0.0.1")
                client.IPv4 = "127.0.0.1";
            client.IPv6 = client.HttpContext.Connection.RemoteIpAddress?.MapToIPv6().ToString() ?? "";//Some error here
            client.Port = client.HttpContext.Request.Host.Port ?? 80;
            client.HttpProtocol = client.HttpContext.Request.Protocol;
            client.HttpScheme = client.HttpContext.Request.Scheme;
            client.RequestedUrl = client.HttpContext.Request.Path;
            client.QueryString = client.HttpContext.Request.QueryString.Value?.Replace("?", "") ?? "";


            var host = client.HttpContext.Request.Host;

            if (host.HasValue)
            {
                if(!IsValidIP(host.Host))
                {
                    var parts = host.Host.Split('.');
                    if (parts.Length > 2)
                    {
                        if (!parts[0].Equals("www", StringComparison.CurrentCultureIgnoreCase))
                        {
                            client.Subdomain = parts[0];
                            
                        }
                        client.Domain = $"{parts[1]}.{parts[2]}";
                    }else
                    {
                        client.Domain = host.Host;
                    }
                }
            }
            return Task.FromResult(client);
        }

        public bool IsValidIP(string ip) => IPAddress.TryParse(ip, out _);
    }
}
