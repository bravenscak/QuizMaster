using QuizMaster.Models;

namespace QuizMaster.Services
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User?> GetUserByIdAsync(int id);
        Task<User?> GetUserByEmailAsync(string email);
        Task<User?> GetUserByUsernameAsync(string username);
        Task<User> RegisterUserAsync(string firstName, string lastName, string email, string username, string password, int roleId, string? organizationName = null, string? description = null);
        Task<User?> LoginAsync(string usernameOrEmail, string password);
        Task<User> UpdateUserAsync(User user);
        Task<bool> DeleteUserAsync(int id);
        Task<bool> EmailExistsAsync(string email);
        Task<bool> UsernameExistsAsync(string username);
    }
}
