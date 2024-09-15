using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Weather.Core.Services;
using Weather.Service.Exceptions;

namespace Weather.API.Controllers
{
    [Route("api")]
    [ApiController]
    public class WeatherController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IWeatherService _weatherService;

        public WeatherController(IUserService userService, IWeatherService weatherService)
        {
            _userService = userService;
            _weatherService = weatherService;
        }

        // Weather method
        [HttpGet("weather/{username}")]
        [Authorize]
        public async Task<IActionResult> GetWeather(string userName)
        {
            var user = await _userService.GetUserByUserNameAsync(userName);
            if (user == null)
            {
                throw new NotFoundExcepiton($"UserName Not Found: {userName}");
            }

            var weather = await _weatherService.GetWeatherDataAsync(user.Address);
            if (weather == null)
            {
                throw new NotFoundExcepiton($"Weather Data Not Found For Address: {user.Address}");
            }
            return Ok(weather);

        }
    }
}
