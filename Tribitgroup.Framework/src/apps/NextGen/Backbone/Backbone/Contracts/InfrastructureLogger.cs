namespace NextGen.Backbone.Backbone.Contracts
{
    public class InfrastructureLogger
    {
        ILogger Logger { get; }

        public InfrastructureLogger(ILogger logger) => Logger = logger;

        public Task LogAsync(string message, object? input = null, object? output = null, string trackingId = "")
            => Logger.LogAsync(message, ApplicationLayerEnum.Infrastructure, input, output, trackingId);

        public Task ErrorAsync(string message, object? input = null, object? output = null, string trackingId = "")
            => Logger.ErrorAsync(message, ApplicationLayerEnum.Infrastructure, input, output, trackingId);

        public Task WarningAsync(string message, object? input = null, object? output = null, string trackingId = "")
            => Logger.WarningAsync(message, ApplicationLayerEnum.Infrastructure, input, output, trackingId);
    }
}