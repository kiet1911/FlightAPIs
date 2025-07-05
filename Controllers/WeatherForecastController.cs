using Microsoft.AspNetCore.Mvc;
using FlightAPIs.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Azure.Core.Serialization;
using Microsoft.AspNetCore.Authorization;
namespace FlightAPIs.Controllers
{

    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize(Roles ="Admin")]
    public class WeatherForecastController : ControllerBase
    {
        DbAbb296Kuphe1980Context db_contex = new DbAbb296Kuphe1980Context();
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpGet]
        public new async Task<TicketManager?> User()
        {

            var dataF = await db_contex.TicketManagers.FindAsync(2);
            if(dataF != null)
            {
                db_contex.Entry(dataF).Reference(x => x.FlightSchedules).Load();

                return dataF;
            }
            return null;

        }


    }
}
