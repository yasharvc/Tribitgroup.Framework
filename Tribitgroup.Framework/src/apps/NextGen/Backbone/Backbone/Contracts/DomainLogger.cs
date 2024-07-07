namespace NextGen.Backbone.Backbone.Contracts
{
    public class DomainLogger
    {
        ILogger Logger { get; }

        public DomainLogger(ILogger logger) => Logger = logger;

        public Task LogAsync(string message, object? input = null, object? output = null, string trackingId = "")
            => Logger.LogAsync(message, ApplicationLayerEnum.Domain, input, output, trackingId);

        public Task ErrorAsync(string message, object? input = null, object? output = null, string trackingId = "")
            => Logger.ErrorAsync(message, ApplicationLayerEnum.Domain, input, output, trackingId);

        public Task WarningAsync(string message, object? input = null, object? output = null, string trackingId = "")
            => Logger.WarningAsync(message, ApplicationLayerEnum.Domain, input, output, trackingId);
    }
}