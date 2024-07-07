using NextGen.Backbone.ServiceProvider.Contracts;

namespace NextGen.Backbone.Backbone.Contracts
{
    public interface IApplicationBackboneProvider : IBackboneProvider
    {
        ApplicationLogger Logger { get; }
        IEventHub EventHub { get; }
    }
}