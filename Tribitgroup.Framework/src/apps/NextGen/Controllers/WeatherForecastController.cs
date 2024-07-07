using Microsoft.AspNetCore.Mvc;
using NextGen.Backbone.Backbone.Contracts;
using NextGen.Backbone.ServiceProvider.Contracts;

namespace NextGen.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly Backbone.Backbone.Contracts.ILogger _logger;
        private readonly IBackboneProvider domainBackbone;

        public WeatherForecastController(
            IBackboneProvider backboneProvider,
            Backbone.Backbone.Contracts.ILogger logger
            )
        {
            _logger = logger;
            domainBackbone = backboneProvider;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public async Task<IEnumerable<WeatherForecast>> Get()
        {
            await _logger.WarningAsync($"{await domainBackbone.GetApplicationMode()}-{(await domainBackbone.GetCurrentUser<ApplicationUser>()).Name}" , ApplicationLayerEnum.Controller);
            var res = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();

            return res;
        }
    }
}
