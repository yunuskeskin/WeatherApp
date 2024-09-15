using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weather.Service.Services;

namespace Weather.Test
{
    [TestFixture]
    public class PasswordServiceTests
    {
        private PasswordService _passwordService;

        [SetUp]
        public void Setup()
        {
            _passwordService = new PasswordService();
        }

        [Test]
        public void GenerateSalt_ShouldReturnUniqueSalt()
        {
            // Act
            var salt1 = _passwordService.GenerateSalt();
            var salt2 = _passwordService.GenerateSalt();

            // Assert
            Assert.That(salt1, Is.Not.EqualTo(salt2)); // Salt must be unique
        }

        [Test]
        public void HashPasswordWithSalt_ShouldReturnHashedPassword()
        {
            // Arrange
            var password = "password123";
            var salt = "randomSalt";

            // Act
            var hashedPassword = _passwordService.HashPasswordWithSalt(password, salt);

            // Assert
            Assert.That(hashedPassword, Is.Not.Null);
        }

        [Test]
        public void VerifyPassword_ShouldReturnTrue_WhenPasswordMatches()
        {
            // Arrange
            var password = "password123";
            var salt = _passwordService.GenerateSalt();
            var hashedPassword = _passwordService.HashPasswordWithSalt(password, salt);

            // Act
            var result = _passwordService.VerifyPassword(password, hashedPassword, salt);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void VerifyPassword_ShouldReturnFalse_WhenPasswordDoesNotMatch()
        {
            // Arrange
            var password = "password123";
            var wrongPassword = "wrongPassword";
            var salt = _passwordService.GenerateSalt();
            var hashedPassword = _passwordService.HashPasswordWithSalt(password, salt);

            // Act
            var result = _passwordService.VerifyPassword(wrongPassword, hashedPassword, salt);

            // Assert
            Assert.That(result, Is.False);
        }
    }
}
