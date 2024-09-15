using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Weather.Core.Services;

namespace Weather.Service.Services
{
    public class PasswordService : IPasswordService
    {
        // Verify if the provided password matches the stored hashed password
        public bool VerifyPassword(string enteredPassword, string storedHashedPassword, string storedSalt)
        {
            // Hash the entered password using the stored salt
            var hashedPassword = HashPasswordWithSalt(enteredPassword, storedSalt);

            // Compare the hashed entered password with the stored hashed password
            return hashedPassword == storedHashedPassword;
        }

        // Hash a password using the provided salt
        public string HashPasswordWithSalt(string password, string salt)
        {
            using (var sha256 = SHA256.Create())
            {
                // Combine password and salt and compute the hash
                var saltedPassword = password + salt;
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword));

                // Convert byte array to a string (hex representation)
                StringBuilder result = new StringBuilder();
                foreach (byte b in bytes)
                {
                    result.Append(b.ToString("x2"));
                }

                return result.ToString();
            }
        }

        // Generate a random salt for password hashing
        public string GenerateSalt(int size = 32)
        {
            var rng = new RNGCryptoServiceProvider();
            var saltBytes = new byte[size];
            rng.GetBytes(saltBytes);

            return Convert.ToBase64String(saltBytes);
        }
    }
}
