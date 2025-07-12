using QuizMaster.DTOs;
using QuizMaster.Models;

namespace QuizMaster.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserResponseDto>> GetAllUsersAsync();
        Task<UserResponseDto?> GetUserByIdAsync(int id);
        Task<User?> GetUserByEmailAsync(string email);
        Task<User?> GetUserByUsernameAsync(string username);
        Task<User> RegisterUserAsync(string firstName, string lastName, string email, string username, string password, int roleId, string? organizationName = null, string? description = null);
        Task<User?> LoginAsync(string usernameOrEmail, string password);
        Task<UserResponseDto?> UpdateUserAsync(int userId, UpdateUserDto updateUserDto);
        Task<bool> DeleteUserAsync(int id);
        Task<bool> EmailExistsAsync(string email);
        Task<bool> UsernameExistsAsync(string username);
        Task ChangePasswordAsync(int userId, ChangePasswordDto changePasswordDto);
    }
}
