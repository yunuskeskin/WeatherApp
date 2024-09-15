using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Weather.Core.DTOs;
using Weather.Core.Services;

namespace Weather.API.Controllers
{
    [Route("api")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ICountryApiService _countryApiService;
        private readonly IUserService _userService;
        private readonly IPasswordService _passwordService;
        private readonly ITokenService _tokenService;

        public UserController(ICountryApiService countryApiService, IUserService userService, IPasswordService passwordService, ITokenService tokenService)
        {
            _countryApiService = countryApiService;
            _userService = userService;
            _passwordService = passwordService;
            _tokenService = tokenService;
        }

        // Registration method
        [HttpPost("registration")]
        public async Task<IActionResult> RegisterAsync(UserRegistrationDto userRegistratonDto)
        {
            var countryValidation = await _countryApiService.ValidateCountryAsync(userRegistratonDto.CountryCitizen);
            if (!countryValidation.IsSuccess)
                throw new ApplicationException("Invalid country");

            var userName = await _userService.RegisterUserAsync(userRegistratonDto);

            return Ok(new { Username = userName });
        }

        // Login method
        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync(UserLoginDto loginDto)
        {
            var user = await _userService.GetUserByUserNameAsync(loginDto.UserName);
            if (user == null)
            {
                return Unauthorized();
            }

            // Verify the entered password using the stored salt
            if (!_passwordService.VerifyPassword(loginDto.Password, user.Password, user.Salt))
            {
                return Unauthorized(); // Incorrect password
            }

            // Generate a JWT token for the user
            var token = _tokenService.GenerateToken(user);

            // Return the token to the client
            return Ok(new { Token = token });
        }
    }
}
