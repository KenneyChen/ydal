using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Repository;
using Repository.Impl;

namespace Samples.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private IDemoRepostiory DemoRepostiory1;


        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IDemoRepostiory DemoRepostiory)
        {
            _logger = logger;
            this.DemoRepostiory1 = DemoRepostiory;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            DemoRepostiory1.Insert(new NetCore_Dal.Models.Demo { UserName = "马虎贴吧222" });

            Console.WriteLine($"{DemoRepostiory1.GetHashCode()}");
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
