using Test.API.Authentication.Contracts;

namespace Test.API.Authentication.Builder.Contracts
{
    public interface IAuthenticatorBuilder<Tenant, Policy, Role, Permission>
        where Role : IRole
        where Permission : IPermission
        where Tenant : ITenant
        where Policy : IPolicy
    {
        Task<IAuthenticatorBuilder<Tenant, Policy, Role, Permission>>
            AddPreauthenticateStepAsync(params IPreauthenticatePipeStep<Tenant, Policy, Role, Permission>[] steps);

        IAuthenticatorBuilder<Tenant, Policy, Role, Permission> 
            AddPreauthenticateStep<T>()
            where T : IPreauthenticatePipeStep<Tenant, Policy, Role, Permission>;

        Task<IAuthenticatorBuilder<Tenant, Policy, Role, Permission>>
            AddAuthenticateStepAsync(params IAuthenticatePipeStep<Tenant, Policy, Role, Permission>[] steps);
            
        Task<IAuthenticatorBuilder<Tenant, Policy, Role, Permission>>
            AddConfigurationAsync(IAuthenticationConfiguration configuration);
        Task<Authenticator<Tenant, Policy, Role, Permission>> BuildAsync();
    }

    public class AuthenticatorBuilder<Tenant, Policy, Role, Permission> : IAuthenticatorBuilder<Tenant, Policy, Role, Permission>
        where Role : IRole
        where Permission : IPermission
        where Tenant : ITenant
        where Policy : IPolicy
    {
        List<IPreauthenticatePipeStep<Tenant, Policy, Role, Permission>> Preauthenticates { get; set; } = new List<IPreauthenticatePipeStep<Tenant, Policy, Role, Permission>>();
        List<IAuthenticatePipeStep<Tenant, Policy, Role, Permission>> AuthenticateSteps { get; set; } = new List<IAuthenticatePipeStep<Tenant, Policy, Role, Permission>>();
        IAuthenticationConfiguration Configuration { get; set; } = new AuthenticationConfiguration();
        IServiceProvider ServiceProvider { get; }

        public AuthenticatorBuilder(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }
        public async Task<IAuthenticatorBuilder<Tenant, Policy, Role, Permission>>
            AddPreauthenticateStepAsync(params IPreauthenticatePipeStep<Tenant, Policy, Role, Permission>[] steps)
        {
            Preauthenticates.AddRange(steps);
            return this;
        }
        public IAuthenticatorBuilder<Tenant, Policy, Role, Permission>
            AddPreauthenticateStep<T>()
            where T : IPreauthenticatePipeStep<Tenant, Policy, Role, Permission>
        {
            Preauthenticates.Add(ServiceProvider.GetRequiredService<T>() ?? throw new Exception());
            return this;
        }
        public async Task<IAuthenticatorBuilder<Tenant, Policy, Role, Permission>>
            AddAuthenticateStepAsync(params IAuthenticatePipeStep<Tenant, Policy, Role, Permission>[] steps)
        {
            AuthenticateSteps.AddRange(steps);
            return this;
        }
        public async Task<IAuthenticatorBuilder<Tenant, Policy, Role, Permission>>
            AddConfigurationAsync(IAuthenticationConfiguration configuration)
        {
            Configuration = configuration;
            return this;
        }
        public async Task<Authenticator<Tenant, Policy, Role, Permission>> BuildAsync() 
            => new Authenticator<Tenant, Policy, Role, Permission>(Configuration, Preauthenticates, AuthenticateSteps);
    }
}
