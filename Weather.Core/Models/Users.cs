using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weather.Core.Models
{
    public class User : BaseEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; } //(Must be with prefix)
        public string Password { get; set; }
        public string Address { get; set; }
        public string BirthDate { get; set; }
        public string LivingCountry { get; set; }
        public string CitizenCountry { get; set; }
        public string Salt { get; set; }
    }
}
