using NextGen.Backbone.ServiceProvider.Contracts;

namespace NextGen.Backbone.Backbone.Contracts
{
    public interface IInfrastructureBackboneProvider : IBackboneProvider
    {
        InfrastructureLogger Logger { get; }
        IEventHub EventHub { get; }
    }
}
