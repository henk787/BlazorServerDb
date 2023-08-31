using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace BlazorServerDb.Data
{
    public class WeatherForecastService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _appDbContextFactory;

        public WeatherForecastService(IDbContextFactory<ApplicationDbContext> appDbContextFactory)
        {
            _appDbContextFactory = appDbContextFactory;
        }

        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };


        public async Task<int> AddForeCasts(DateTime startDate)
        {
            var newForecasts = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = startDate.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            }).ToArray();

            await using var dbContext = await _appDbContextFactory.CreateDbContextAsync();

            dbContext.WeatherForecasts.AddRange(newForecasts);

            return await dbContext.SaveChangesAsync();
        }

        public async Task<long> CountForeCasts()
        {
            await using var dbContext = await _appDbContextFactory.CreateDbContextAsync();
            return await dbContext.WeatherForecasts.LongCountAsync();
        }

        public async Task<WeatherForecast[]> GetForecastAsync()
        {
            await using var dbContext = await _appDbContextFactory.CreateDbContextAsync();

            var results = await dbContext.WeatherForecasts
                .OrderByDescending(w => w.Date)
                .Take(20)
                .ToArrayAsync();

            return results;
        }
    }
}