using Microsoft.Extensions.Caching.Memory;
using Moq.Protected;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Weather.Core.DTOs;
using Weather.Core.Models;
using Weather.Service.Exceptions;
using Weather.Service.Services;

namespace Weather.Test
{
    [TestFixture]
    public class WeatherServiceTests
    {
        private Mock<IHttpClientFactory> _httpClientFactoryMock;
        private Mock<IMemoryCache> _memoryCacheMock;
        private WeatherService _weatherService;

        [SetUp]
        public void Setup()
        {
            _httpClientFactoryMock = new Mock<IHttpClientFactory>();
            _memoryCacheMock = new Mock<IMemoryCache>();

            _weatherService = new WeatherService(_httpClientFactoryMock.Object, _memoryCacheMock.Object);
        }

        [Test]
        public async Task GetWeatherDataAsync_ShouldReturnWeatherData_WhenApiCallIsSuccessful()
        {
            // Arrange
            var userAddress = "London";
            var weatherApiResponse = new OpenWeatherMapResponseDto
            {
                Name = "London",
                Main = new MainData { Temp = 15 },
                Weather = new List<Weather.Core.Models.Weather> { new Weather.Core.Models.Weather { Description = "Clear sky" } }
            };

            var responseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(weatherApiResponse))
            };

            var httpClientMock = new Mock<HttpMessageHandler>();
            httpClientMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(responseMessage);

            var client = new HttpClient(httpClientMock.Object);

            _httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(client);

            // Mock memory cache for TryGetValue (returns false to simulate cache miss)
            _memoryCacheMock.Setup(m => m.TryGetValue(It.IsAny<object>(), out It.Ref<object>.IsAny)).Returns(false);

            // Mock memory cache for Set (simulate caching the value)
            var mockCacheEntry = Mock.Of<ICacheEntry>();
            _memoryCacheMock.Setup(m => m.CreateEntry(It.IsAny<object>())).Returns(mockCacheEntry);

            // Act
            var result = await _weatherService.GetWeatherDataAsync(userAddress);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.City, Is.EqualTo("London"));
            Assert.That(result.Temperature, Is.EqualTo("15°C"));
            Assert.That(result.Condition, Is.EqualTo("Clear sky"));
        }

        [Test]
        public async Task GetWeatherDataAsync_ShouldReturnCachedWeatherData_WhenDataIsInCache()
        {
            // Arrange
            var userAddress = "London";
            var cachedWeather = new WeatherData
            {
                City = "London",
                Temperature = "20°C",
                Condition = "Partly cloudy",
                LastUpdated = DateTime.Now
            };

            object cacheEntry = cachedWeather;

            _memoryCacheMock.Setup(m => m.TryGetValue(It.IsAny<object>(), out cacheEntry)).Returns(true);

            // Act
            var result = await _weatherService.GetWeatherDataAsync(userAddress);

            // Assert
            Assert.That(result, Is.EqualTo(cachedWeather));
            _httpClientFactoryMock.Verify(x => x.CreateClient(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void GetWeatherDataAsync_ShouldThrowClientSideException_WhenApiCallFails()
        {
            // Arrange
            var userAddress = "InvalidCity";

            var httpClientMock = new Mock<HttpMessageHandler>();
            httpClientMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new HttpRequestException("Network error"));

            var client = new HttpClient(httpClientMock.Object);

            _httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(client);

            _memoryCacheMock.Setup(m => m.TryGetValue(It.IsAny<object>(), out It.Ref<object>.IsAny)).Returns(false);

            // Act & Assert
            var ex = Assert.ThrowsAsync<ClientSideException>(async () => await _weatherService.GetWeatherDataAsync(userAddress));
            Assert.That(ex.Message, Is.EqualTo("Error fetching weather data: Network error"));
        }

        [Test]
        public async Task GetWeatherDataAsync_ShouldCacheWeatherData_WhenApiCallIsSuccessful()
        {
            // Arrange
            var userAddress = "New York";
            var weatherApiResponse = new OpenWeatherMapResponseDto
            {
                Name = "New York",
                Main = new MainData { Temp = 10 },
                Weather = new List<Weather.Core.Models.Weather> { new Weather.Core.Models.Weather { Description = "Cloudy" } }
            };

            var responseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(weatherApiResponse))
            };

            var httpClientMock = new Mock<HttpMessageHandler>();
            httpClientMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(responseMessage);

            var client = new HttpClient(httpClientMock.Object);

            _httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(client);

            _memoryCacheMock.Setup(m => m.TryGetValue(It.IsAny<object>(), out It.Ref<object>.IsAny)).Returns(false);

            var cacheEntry = Mock.Of<ICacheEntry>();
            _memoryCacheMock.Setup(m => m.CreateEntry(It.IsAny<object>())).Returns(cacheEntry);

            // Act
            var result = await _weatherService.GetWeatherDataAsync(userAddress);

            // Assert
            Assert.That(result.City, Is.EqualTo("New York"));
            Assert.That(result.Temperature, Is.EqualTo("10°C"));
            Assert.That(result.Condition, Is.EqualTo("Cloudy"));

            // Verify that CreateEntry was called for caching the data
            _memoryCacheMock.Verify(m => m.CreateEntry(It.IsAny<object>()), Times.Once);
        }
    }
}
