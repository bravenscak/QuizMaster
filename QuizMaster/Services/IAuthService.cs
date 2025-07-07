using QuizMaster.DTOs;

namespace QuizMaster.Services
{
    public interface IAuthService
    {
        string GenerateJwtToken(int userId, string username, string email, string roleName);
        Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);
        Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
    }
}
