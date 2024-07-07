using System.Security.Cryptography;
using System.Text;
using Test.API.Authentication.Contracts;

namespace Test.API.Test
{
    public class TokenHasher : HttpPreauthenticatePipeStep<Tenant, Policy, Role, Permission>
    {
        public override Task<HttpClient<Tenant, Policy, Role, Permission>> ExecuteAsync(HttpClient<Tenant, Policy, Role, Permission> client, IAuthenticationConfiguration configuration)
        {
            client.ClientToken = HashToken(client.ClientToken);
            client.ApplicationToken = HashToken(client.ApplicationToken);
            return Task.FromResult(client);
        }

        private string HashToken(string clientToken)
        {
            byte[] data = SHA512.HashData(Encoding.UTF8.GetBytes(clientToken));
            StringBuilder sBuilder = new();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }
    }
}
