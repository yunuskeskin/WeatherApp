using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weather.Core.Models
{
    public class WeatherData
    {
        public string City { get; set; }
        public string Temperature { get; set; }
        public string Condition { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
