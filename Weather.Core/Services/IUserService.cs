using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weather.Core.DTOs;
using Weather.Core.Models;

namespace Weather.Core.Services
{
    public interface IUserService
    {
        Task<string> RegisterUserAsync(UserRegistrationDto userRegistrationDto);
        Task<User> GetUserByUserNameAsync(string userName);
        Task<string> GenerateUniqueUserNameAsync(string firstName, string lastName);
    }
}
