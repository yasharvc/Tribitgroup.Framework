namespace NextGen.Backbone.Backbone.Contracts
{
    public interface ILogger
    {
        Task LogAsync(string message, ApplicationLayerEnum applicationLayer, object? input = null, object? output = null , string trackingId = "");
        Task ErrorAsync(string message, ApplicationLayerEnum applicationLayer, object? input = null, object? output = null, string trackingId = "");
        Task WarningAsync(string message, ApplicationLayerEnum applicationLayer, object? input = null, object? output = null, string trackingId = "");
    }

    public class ConsoleLogger : ILogger
    {
        public Task LogAsync(string message, ApplicationLayerEnum applicationLayer, object? input = null, object? output = null, string trackingId = "")
            => ConsoleWrtieAsync(message, ConsoleColor.White);

        public Task ErrorAsync(string message, ApplicationLayerEnum applicationLayer, object? input = null, object? output = null, string trackingId = "")
            => ConsoleWrtieAsync(message, ConsoleColor.Red);

        public Task WarningAsync(string message, ApplicationLayerEnum applicationLayer, object? input = null, object? output = null, string trackingId = "")
            => ConsoleWrtieAsync(message, ConsoleColor.Yellow);

        Task ConsoleWrtieAsync(string message, ConsoleColor color)
        {
            var temp = Console.ForegroundColor;
            Console.ForegroundColor = color;
            var res = Console.Out.WriteLineAsync($"{DateTime.UtcNow:F}: {message}");
            Console.ForegroundColor = temp;
            return res;
        }
    }
}