using NextGen.Backbone.ServiceProvider.Contracts;

namespace NextGen.Backbone.Backbone.Contracts
{
    public interface IDomainBackboneProvider : IBackboneProvider
    {
        DomainLogger Logger { get; }
        IEventHub EventHub { get; }
    }
}