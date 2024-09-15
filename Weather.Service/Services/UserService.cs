using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weather.Core.DTOs;
using Weather.Core.Models;
using Weather.Core.Repositories;
using Weather.Core.Services;
using Weather.Core.UnitOfWorks;

namespace Weather.Service.Services
{
    public class UserService : Service<User>, IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordService _passwordService;

        public UserService(IGenericRepository<User> repository, IUnitOfWork unitOfWork, IUserRepository userRepository, IMapper mapper, IPasswordService passwordService) : base(repository, unitOfWork)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _passwordService = passwordService;
        }

        public async Task<string> GenerateUniqueUserNameAsync(string firstName, string lastName)
        {
            //Generate the base username
            string baseUsername = $"{firstName.ToLower()[0]}{lastName.ToLower()}";

            //Check if the username already exists
            var existingUsers = await _userRepository.GetAll()
                .Where(u => u.UserName.StartsWith(baseUsername))
                .Select(u => u.UserName)
                .ToListAsync();

            //If no one has this username, return it directly
            if (!existingUsers.Any())
            {
                return baseUsername;
            }

            //Find the highest number among existing usernames
            int maxNumber = existingUsers
                .Where(u => u.Length > baseUsername.Length)  //Get usernames with a number appended
                .Select(u => u.Substring(baseUsername.Length))  //Extract the numeric part
                .Where(num => int.TryParse(num, out _))  //Filter out non-numeric parts
                .Select(int.Parse)  //Convert string to integer
                .DefaultIfEmpty(0)  //If no number is appended, default to 0
                .Max();  //Get the maximum number

            // Generate a new unique username
            return $"{baseUsername}{maxNumber + 1}";
        }

        public async Task<string> RegisterUserAsync(UserRegistrationDto userDto)
        {
            var userName = await GenerateUniqueUserNameAsync(userDto.FirstName, userDto.LastName);
            var salt = _passwordService.GenerateSalt();

            // Hash the user's password with the salt
            string hashedPassword = _passwordService.HashPasswordWithSalt(userDto.Password, salt);

            DateTime birthDate;
            string format = "dd/MM/yyyy";
            bool convertingGivenDateToDateTimeIsSuccessfull = DateTime.TryParseExact(userDto.BirthDate, format,
                                                  System.Globalization.CultureInfo.InvariantCulture,
                                                  System.Globalization.DateTimeStyles.None,
                                                  out birthDate);

            var newUser = new User
            {
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                UserName = userName,
                Email = userDto.Email,
                Password = hashedPassword,
                LivingCountry = userDto.LivingCountry,
                CitizenCountry = userDto.CountryCitizen,
                PhoneNumber = userDto.PhoneNumber,
                Address = userDto.Address,
                BirthDate = convertingGivenDateToDateTimeIsSuccessfull ? userDto.BirthDate : "",
                Salt = salt
            };

            await _userRepository.AddAsync(newUser);
            await _unitOfWork.CommitAsync();
            return userName;
        }

        public async Task<User> GetUserByUserNameAsync(string userName)
        {
            return await _userRepository.Where(x => x.UserName == userName).SingleOrDefaultAsync(); ;
        }
    }
}
