using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Weather.Core.DTOs;
using Weather.Core.Models;
using Weather.Core.Services;
using Weather.Service.Exceptions;

namespace Weather.Service.Services
{
    public class WeatherService : IWeatherService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMemoryCache _memoryCache;
        private readonly string _apiKey = "YOUR-API-KEY-HERE"; // OpenWeatherMap API key
        private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(60);  // Cache duration
        public WeatherService(IHttpClientFactory httpClientFactory, IMemoryCache memoryCache)
        {
            _httpClientFactory = httpClientFactory;
            _memoryCache = memoryCache;
        }

        public async Task<WeatherData> GetWeatherDataAsync(string userAddress)
        {
            var cacheKey = $"weather_forecast_{userAddress}";
            // Check if weather data is cached
            if (!_memoryCache.TryGetValue(cacheKey, out WeatherData cachedWeather))
            {
                // Data is not in cache, so fetch it from OpenWeatherMap API
                var client = _httpClientFactory.CreateClient();
                var apiUrl = $"http://api.openweathermap.org/data/2.5/weather?q={userAddress}&appid={_apiKey}&units=metric";

                try
                {
                    var response = await client.GetAsync(apiUrl);
                    if (response.IsSuccessStatusCode)
                    {
                        // Deserialize the JSON response into WeatherData object
                        var weatherApiResponse = await response.Content.ReadFromJsonAsync<OpenWeatherMapResponseDto>();

                        // Convert the API response to WeatherData format
                        var weatherData = new WeatherData
                        {
                            City = weatherApiResponse.Name,
                            Temperature = $"{weatherApiResponse.Main.Temp}°C",
                            Condition = weatherApiResponse.Weather[0].Description,
                            LastUpdated = DateTime.Now
                        };

                        // Cache the data
                        var cacheEntryOptions = new MemoryCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = _cacheDuration,
                            SlidingExpiration = TimeSpan.FromMinutes(5)
                        };

                        _memoryCache.Set(cacheKey, weatherData, cacheEntryOptions);

                        cachedWeather = weatherData;
                    }
                    else
                    {
                        throw new ClientSideException("Error fetching weather data from OpenWeatherMap API");
                    }
                }
                catch (HttpRequestException ex)
                {
                    // Handle the error in case of network issues or external service failure
                    throw new ClientSideException($"Error fetching weather data: {ex.Message}");
                }
            }
            return cachedWeather;
        }
    }
}
