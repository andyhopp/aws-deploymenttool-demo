const string DynamoDbTableName = "DeploymentToolWeatherTable";
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors(o => o
                .AllowAnyMethod()
                .AllowAnyHeader()
                .SetIsOriginAllowed(origin => 
                    builder.Configuration["AllowedOrigins"] == "*" || 
                    builder.Configuration["AllowedOrigins"].Split(";").Contains(origin)));

// Below is our sample REST API

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/api/weatherforecast", async () =>
{
    var ddbClient = new Amazon.DynamoDBv2.AmazonDynamoDBClient();
    var weatherForecastTable = Amazon.DynamoDBv2.DocumentModel.Table.LoadTable(
        ddbClient,
        new Amazon.DynamoDBv2.DocumentModel.TableConfig(DynamoDbTableName));
    var search = weatherForecastTable.Scan(new Amazon.DynamoDBv2.DocumentModel.ScanOperationConfig {});
    var items = await search.GetRemainingAsync();
    var forecast = items.Select(item =>
       new WeatherForecast
       (
           DateTime.Parse(item["date"].AsString()),
           item["temperatureC"].AsInt(),
           item["summary"].AsString()
       ));
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

internal record WeatherForecast(DateTime Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}