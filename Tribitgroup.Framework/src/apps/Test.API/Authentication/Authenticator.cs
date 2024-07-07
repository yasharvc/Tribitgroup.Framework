using Test.API.Authentication.Contracts;

namespace Test.API.Authentication
{
    public class Authenticator<Tenant, Policy, Role, Permission>
        where Role : IRole
        where Permission : IPermission
        where Tenant : ITenant
        where Policy : IPolicy
    {
        IEnumerable<IPreauthenticatePipeStep<Tenant, Policy, Role, Permission>> Preauthenticates { get; } = new List<IPreauthenticatePipeStep<Tenant, Policy, Role, Permission>>();
        IEnumerable<IAuthenticatePipeStep<Tenant, Policy, Role, Permission>> AuthenticateSteps { get; } = new List<IAuthenticatePipeStep<Tenant, Policy, Role, Permission>>();
        IAuthenticationConfiguration Configuration { get; }
        public Authenticator(
            IAuthenticationConfiguration configuration,
            IEnumerable<IPreauthenticatePipeStep<Tenant, Policy, Role, Permission>> preauthenticates,
            IEnumerable<IAuthenticatePipeStep<Tenant, Policy, Role, Permission>> authenticateSteps)
        {
            Configuration = configuration;
            Preauthenticates = preauthenticates;
            AuthenticateSteps = authenticateSteps;
        }
        public async Task ProcessAsync(IClient<Tenant, Policy, Role, Permission> client)
        {
            foreach(var step in Preauthenticates)
            {
                try
                {
                    client = await step.ExecuteAsync(client, Configuration);
                }
                catch
                {
                    throw;
                }
            }

            foreach (var step in AuthenticateSteps)
            {
                try
                {
                    client = await step.ExecuteAsync(client, Configuration);
                }
                catch
                {
                    throw;
                }
            }
        }
    }
}
