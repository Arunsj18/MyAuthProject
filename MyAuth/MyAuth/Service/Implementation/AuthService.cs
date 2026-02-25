using MyAuth.DTOs;
using MyAuth.Model.DTO;
using MyAuth.Models;
using MyAuth.Repositories;
using System.Security.Cryptography;
using System.Text;

namespace MyAuth.Services
{
    public class AuthService : IAuthService
    {
        private readonly AuthRepository _userRepository;

        public AuthService(AuthRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<string> RegisterAsync(RegisterRequestDto dto)
        {
            var existingUser = await _userRepository.GetByEmailAsync(dto.Email);
            if (existingUser != null)
                return "Email already exists.";

            CreatePasswordHash(dto.Password, out string hash, out string salt);

            var user = new User
            {
                UserName = dto.UserName,
                Email = dto.Email,
                PasswordHash = hash,
                PasswordSalt = salt
            };

            await _userRepository.AddUserAsync(user);

            return "User registered successfully.";
        }

        private void CreatePasswordHash(string password, out string hash, out string salt)
        {
            using var hmac = new HMACSHA256();
            salt = Convert.ToBase64String(hmac.Key);
            hash = Convert.ToBase64String(
                hmac.ComputeHash(Encoding.UTF8.GetBytes(password)));
        }

        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto dto)
        {
            var user = await _userRepository.GetByEmailAsync(dto.Email);

            if (user == null || !user.IsActive)
                return null;

            using var hmac = new HMACSHA256(
                Convert.FromBase64String(user.PasswordSalt));

            var computedHash = Convert.ToBase64String(
                hmac.ComputeHash(Encoding.UTF8.GetBytes(dto.Password)));

            if (computedHash != user.PasswordHash)
                return null;

            return new LoginResponseDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email
            };
        }
    }
}