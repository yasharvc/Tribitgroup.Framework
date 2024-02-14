using Test.API.Authentication.Contracts;
using Test.API.Authentication.Enums;

namespace Test.API.Test
{
    public class DeviceTypeSetter : HttpPreauthenticatePipeStep<Tenant, Policy, Role, Permission>
    {

        public override Task<HttpClient<Tenant, Policy, Role, Permission>> ExecuteAsync(HttpClient<Tenant, Policy, Role, Permission> httpClient, IAuthenticationConfiguration configuration)
        {
            var deviceTypeHeader = httpClient.HttpContext.Request.Headers["DeviceType"].FirstOrDefault() ?? "";

            httpClient.DeviceType = deviceTypeHeader.ToLower() switch
            {
                "windows" => DeviceTypeEnum.Windows,
                "macos" => DeviceTypeEnum.MacOS,
                "linux" => DeviceTypeEnum.Linux,
                "application" => DeviceTypeEnum.Application,
                "android" => DeviceTypeEnum.Android,
                _ => DeviceTypeEnum.Web
            };

            return Task.FromResult(httpClient);
        }
    }
}
