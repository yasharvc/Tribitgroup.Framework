namespace NextGen.Backbone.Backbone.Contracts
{
    public interface IEventHub
    {
        void Subscribe(Action<string> handler);
        void Unsubscribe(Action<string> handler);
        void Publish(string eventMessage);
    }
}