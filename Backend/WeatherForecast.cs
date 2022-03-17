using System;

namespace Backend
{
    public class WeatherForecast
    {
        public WeatherForecast(DateTime Date, int TemperatureC, string Summary)
        {
            this.Date = Date;
            this.TemperatureC = TemperatureC;
            this.Summary = Summary;
        }

        public DateTime Date { get; set; }
        public int TemperatureC { get; set; }
        public string Summary { get; set; }
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    }
}
