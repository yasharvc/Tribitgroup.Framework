using NextGen.Backbone.Backbone.Contracts;

namespace NextGen.Backbone.ServiceProvider.Contracts
{
    public interface IBackboneProvider
    {
        Dictionary<string, IDBConnectionProvider> DbConnectionProvider { get; }
        CancellationToken CancellationToken { get; }//Get from 
        Task OnApplicationModeChanged(ApplicationModeEnum newApplicationMode);
        Task<T> GetConfiguration<T>() where T : IApplicationConfiguration;
        Task<T> GetCurrentUser<T>() where T : IApplicationUser;
        Task<T?> GetService<T>();
        Task<IServiceProvider> GetServiceProvider();
        string TrackingID { get; }
        Task<ApplicationModeEnum> GetApplicationMode();
    }
}