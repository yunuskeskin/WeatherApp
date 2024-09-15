using AutoMapper;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Weather.Core.DTOs;
using Weather.Core.Models;
using Weather.Core.Repositories;
using Weather.Core.Services;
using Weather.Core.UnitOfWorks;
using Weather.Service.Services;

namespace Weather.Test
{
    [TestFixture]
    public class UserServiceTests
    {
        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<IMapper> _mapperMock;
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<IPasswordService> _passwordServiceMock;
        private UserService _userService;

        [SetUp]
        public void Setup()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _mapperMock = new Mock<IMapper>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _passwordServiceMock = new Mock<IPasswordService>();

            _userService = new UserService(
                new Mock<IGenericRepository<User>>().Object,
                _unitOfWorkMock.Object,
                _userRepositoryMock.Object,
                _mapperMock.Object,
                _passwordServiceMock.Object
            );
        }

        [Test]
        public async Task RegisterUserAsync_ShouldReturnUserName_WhenRegistrationIsSuccessful()
        {
            // Arrange
            var userDto = new UserRegistrationDto
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Password = "password123",
                CountryCitizen = "USA",
                LivingCountry = "USA",
                PhoneNumber = "1234567890",
                Address = "123 Main St",
                BirthDate = "01/01/1990"
            };

            var passwordService = new PasswordService();
            var salt = passwordService.GenerateSalt();
            var hashedPassword = passwordService.HashPasswordWithSalt(userDto.Password, salt);

            _userRepositoryMock.Setup(r => r.GetAll()).Returns(new List<User>().AsQueryable());
            _userRepositoryMock.Setup(r => r.AddAsync(It.IsAny<User>())).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.CommitAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _userService.RegisterUserAsync(userDto);

            // Assert
            Assert.That(result, Is.EqualTo("jdoe")); // Assuming the first user with this name
            _userRepositoryMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Test]
        public async Task GenerateUniqueUserNameAsync_ShouldGenerateUniqueUserName_WhenSimilarUserNamesExist()
        {
            // Arrange
            var existingUsers = new List<User>
            {
                new User
                {
                    UserName = "jdoe1",
                    FirstName = "John",
                    LastName = "Doe",
                    CitizenCountry = "TUR",
                    LivingCountry = "MLT",
                    Address = "Ankara",
                    Email = "temp@gmail.com",
                    BirthDate = "01/01/1990",
                    PhoneNumber = "+905308291478"
                },
                new User
                {
                    UserName = "jdoe2",
                    FirstName = "John",
                    LastName = "Doe",
                    CitizenCountry = "TUR",
                    LivingCountry = "MLT",
                    Address = "Sinop",
                    Email = "temp1@gmail.com",
                    BirthDate = "01/01/1995",
                    PhoneNumber = "+905308291478"
                }
            }.AsQueryable();

            _userRepositoryMock.Setup(r => r.GetAll()).Returns(existingUsers.AsQueryable());

            // Act
            var userName = await _userService.GenerateUniqueUserNameAsync("John", "Doe");

            // Assert
            Assert.That(userName, Is.EqualTo("jdoe3"));
        }
        [Test]
        public async Task GetUserByUserNameAsync_ShouldReturnUser_WhenUserNameExists()
        {
            // Arrange
            var user = new User
            {
                UserName = "jdoe",
                FirstName = "John",
                LastName = "Doe",
                CitizenCountry = "",
                LivingCountry = "",
                Address = "",
                Email = "",
                BirthDate = "",
                Password = "",
                PhoneNumber = "+905308291478"
            };

            _userRepositoryMock.Setup(r => r.Where(It.IsAny<Expression<Func<User, bool>>>()))
                               .Returns(new List<User> { user }.AsQueryable());

            // Act
            var result = await _userService.GetUserByUserNameAsync("jdoe");

            // Assert
            Assert.That(result.UserName, Is.EqualTo("jdoe"));
            Assert.That(result.FirstName, Is.EqualTo("John"));
        }
    }
}
