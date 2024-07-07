using NextGen.Backbone.Backbone.Contracts;
using NextGen.Backbone.ServiceProvider.Contracts;

namespace NextGen.Backbone.Backbone
{
    internal class BackboneProvider : IBackboneProvider
    {
        private readonly ApplicationModeProvider _applicationModeProvider;

        public Dictionary<string, IDBConnectionProvider> DbConnectionProvider { get; } = [];

        public CancellationToken CancellationToken { get; set; } = new CancellationToken();

        public string TrackingID { get; set; } = string.Empty;

        public event EventHandler<ApplicationModeEnum> ApplicationModeChanged = delegate { };
        protected Dictionary<Type, IConfiguration> Configurations { get; set; } = [];
        protected IApplicationUser ApplicationUser { get; set; } = null!;
        public IServiceProvider ServiceProvider { get; }

        public BackboneProvider(
            IApplicationUser applicationUser,
            IServiceProvider serviceProvider,
            ApplicationModeProvider applicationModeProvider
            )
        {
            ApplicationUser = applicationUser;
            ServiceProvider = serviceProvider;
            _applicationModeProvider = applicationModeProvider;
        }

        public Task<ApplicationModeEnum> GetApplicationMode() => Task.FromResult(_applicationModeProvider.ApplicationMode);

        public Task<T> GetConfiguration<T>() where T : IApplicationConfiguration 
            => Task.FromResult((T)Configurations[typeof(T)]);

        public Task<T> GetCurrentUser<T>() where T : IApplicationUser 
            => Task.FromResult((T)ApplicationUser);

        public Task<T?> GetService<T>() 
            => Task.FromResult(ServiceProvider.GetService<T>());

        public Task<IServiceProvider> GetServiceProvider() 
            => Task.FromResult(ServiceProvider);

        public Task OnApplicationModeChanged(ApplicationModeEnum newApplicationMode)
        {
            _applicationModeProvider.SetApplicationMode(newApplicationMode);
            return Task.CompletedTask;
        }

    }

    public class ApplicationModeProvider
    {
        public ApplicationModeEnum ApplicationMode { get; set; } = ApplicationModeEnum.Running;

        public void SetApplicationMode(ApplicationModeEnum applicationMode)
        {
            ApplicationMode = applicationMode;
        }
    }
}
