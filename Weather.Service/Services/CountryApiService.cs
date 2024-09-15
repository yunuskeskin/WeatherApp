using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weather.Core.DTOs;
using Weather.Core.Services;

namespace Weather.Service.Services
{
    public class CountryApiService : ICountryApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl = "https://restcountries.com/v3.1/alpha?codes=";

        public CountryApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<CountryValidationResultDto> ValidateCountryAsync(string countryCode)
        {
            var response = await _httpClient.GetAsync($"{_apiUrl}{countryCode}");
            if (!response.IsSuccessStatusCode)
                return new CountryValidationResultDto { IsSuccess = false };

            var countryData = await response.Content.ReadAsStringAsync();
            // Burada countryData işlenir ve sonuç döndürülür
            return new CountryValidationResultDto { IsSuccess = true };
        }
    }
}
