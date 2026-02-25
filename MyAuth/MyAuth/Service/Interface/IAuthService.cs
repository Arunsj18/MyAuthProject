using MyAuth.DTOs;
using MyAuth.Model.DTO;

namespace MyAuth.Services
{
    public interface IAuthService
    {
        Task<string> RegisterAsync(RegisterRequestDto dto);
        Task<LoginResponseDto> LoginAsync(LoginRequestDto dto);

    }
}