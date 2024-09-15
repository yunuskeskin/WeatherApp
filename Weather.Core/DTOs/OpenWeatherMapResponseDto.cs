using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Weather.Core.Models;

namespace Weather.Core.DTOs
{
    public class OpenWeatherMapResponseDto
    {
        public string Name { get; set; } // City name
        public MainData Main { get; set; }
        public List<Models.Weather> Weather { get; set; }
    }
}
