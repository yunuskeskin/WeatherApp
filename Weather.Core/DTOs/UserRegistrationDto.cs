using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weather.Core.Validations;

namespace Weather.Core.DTOs
{
    public class UserRegistrationDto
    {
        [Required(ErrorMessage = "First name is required")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Country Citizen is required")]
        [MaxLength(3, ErrorMessage = "Country Citizen must be at least 3 letters")]
        public string CountryCitizen { get; set; }

        [Required(ErrorMessage = "Living Country is required")]
        [MaxLength(3, ErrorMessage = "Living Citizen must be at least 3 letters")]
        public string LivingCountry { get; set; }

        [PhoneNumber]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Address is required")]
        public string Address { get; set; }
        public string BirthDate { get; set; }
    }
}
