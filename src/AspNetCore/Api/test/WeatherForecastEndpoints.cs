using Microsoft.AspNetCore.Http.HttpResults;
using System.Text.Json;

public static class WeatherForecastEndpoints
{
    public static void RegisterWeatherForecastEndpoints(this WebApplication app)
    {
        app.MapGet("/api/test/weatherforecast", GetWeatherForecast)
            .WithName("GetWeatherForecast");
    }

    static string GetWeatherForecast()
    {
        var summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();

        string json = JsonSerializer.Serialize(forecast, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        return json;
    }

    internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
    {
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    }
}
