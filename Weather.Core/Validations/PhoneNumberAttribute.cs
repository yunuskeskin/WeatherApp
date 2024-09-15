using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weather.Core.Validations
{
    public class PhoneNumberAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // Check if the value is null or empty
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return new ValidationResult("Phone number is required.");
            }

            string phoneNumber = value.ToString();

            // Check if the phone number starts with '+'
            if (!phoneNumber.StartsWith("+"))
            {
                return new ValidationResult("Phone number must start with a '+' prefix.");
            }

            return ValidationResult.Success;
        }
    }
}
