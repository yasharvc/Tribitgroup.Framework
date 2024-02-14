using System.Net.Http;
using Test.API.Authentication.Contracts;
using Test.API.Authentication.Enums;

namespace Test.API.Test
{
    public class HttpCommandSetter : HttpPreauthenticatePipeStep<Tenant, Policy, Role, Permission>
    {
        public override Task<HttpClient<Tenant, Policy, Role, Permission>> ExecuteAsync(HttpClient<Tenant, Policy, Role, Permission> httpClient, IAuthenticationConfiguration configuration)
        {
            var httpCommand = httpClient.HttpContext.Request.Method.ToUpper();

            if (httpCommand == "GET")
                httpClient.HttpCommand = HttpCommandEnum.GET;
            else if (httpCommand == "POST")
                httpClient.HttpCommand = HttpCommandEnum.POST;
            else if (httpCommand == "PUT")
                httpClient.HttpCommand = HttpCommandEnum.PUT;
            else if (httpCommand == "DELETE")
                httpClient.HttpCommand = HttpCommandEnum.DELETE;
            else if (httpCommand == "PATCH")
                httpClient.HttpCommand = HttpCommandEnum.PATCH;
            else if (httpCommand == "HEAD")
                httpClient.HttpCommand = HttpCommandEnum.HEAD;
            else if (httpCommand == "OPTIONS")
                httpClient.HttpCommand = HttpCommandEnum.OPTIONS;
            else
                httpClient.HttpCommand = HttpCommandEnum.Unknown;
            return Task.FromResult(httpClient);
        }
    }
}