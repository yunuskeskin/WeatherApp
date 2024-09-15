using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weather.Core.DTOs;

namespace Weather.Core.Services
{
    public interface ICountryApiService
    {
        Task<CountryValidationResultDto> ValidateCountryAsync(string country);
    }
}
