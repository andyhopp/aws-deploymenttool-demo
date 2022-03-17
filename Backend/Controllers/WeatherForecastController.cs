using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        const string DynamoDbTableName = "DeploymentToolWeatherTable";

        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<IEnumerable<WeatherForecast>> Get()
        {
            var ddbClient = new Amazon.DynamoDBv2.AmazonDynamoDBClient();
            var weatherForecastTable = Amazon.DynamoDBv2.DocumentModel.Table.LoadTable(
                ddbClient,
                new Amazon.DynamoDBv2.DocumentModel.TableConfig(DynamoDbTableName));
            var search = weatherForecastTable.Scan(new Amazon.DynamoDBv2.DocumentModel.ScanOperationConfig { });
            var items = await search.GetRemainingAsync();
            var forecast = items.Select(item =>
               new WeatherForecast
               (
                   DateTime.Parse(item["date"].AsString()),
                   item["temperatureC"].AsInt(),
                   item["summary"].AsString()
               ));
            return forecast;
        }
    }
}
