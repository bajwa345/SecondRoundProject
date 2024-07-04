using System;
using System.Text;
using System.Security.Claims;
using FluentValidation;
using FluentValidation.Results;
using SecondRoundProject.DTOs;
using SecondRoundProject.Models;
using SecondRoundProject.Repositories;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using SecondRoundProject.Helpers;

namespace SecondRoundProject.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IValidator<RegisterDTO> _registerValidator;
        private readonly IValidator<LoginDTO> _loginValidator;
        private readonly IConfiguration _configuration;
        private readonly ILogger<UserService> _logger;

        public UserService(
            IUserRepository userRepository,
            IValidator<RegisterDTO> registerValidator,
            IValidator<LoginDTO> loginValidator,
            IConfiguration configuration,
            ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _registerValidator = registerValidator;
            _loginValidator = loginValidator;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<(bool, string)> RegisterAsync(RegisterDTO model)
        {
            ValidationResult validationResult = await _registerValidator.ValidateAsync(model);
            if (!validationResult.IsValid)
            {
                return (false, string.Join(", ", validationResult.Errors));
            }

            var userExists = await _userRepository.GetUserByUsernameAsync(model.Username);
            if (userExists != null)
            {
                return (false, "User already exists.");
            }

            var user = new ApplicationUser
            {
                Username = model.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                FirstName = model.FirstName,
                LastName = model.LastName,
                Role = model.Role
            };

            var result = await _userRepository.CreateUserAsync(user);
            if (result == 0)
            {
                return (false, "User creation failed.");
            }

            _logger.LogInformation("User created successfully: {Username}", model.Username);
            return (true, "User created successfully.");
        }

        public async Task<(bool, string, string?)> LoginAsync(LoginDTO model)
        {
            ValidationResult validationResult = await _loginValidator.ValidateAsync(model);
            if (!validationResult.IsValid)
            {
                return (false, string.Join(", ", validationResult.Errors), null);
            }

            var user = await _userRepository.GetUserByUsernameAsync(model.Username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
            {
                return (false, "Invalid username or password.", null);
            }

            var token = JwtHelper.GenerateJwtToken(user, _configuration);
            return (true, "Login successful.", token);
        }
    }

}
