using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weather.Core.Services
{
    public interface IPasswordService
    {
        bool VerifyPassword(string enteredPassword, string storedHashedPassword, string storedSalt);
        string HashPasswordWithSalt(string password, string salt);
        string GenerateSalt(int size = 32);

    }
}
