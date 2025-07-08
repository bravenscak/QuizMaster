using AutoMapper;
using QuizMaster.DTOs;
using QuizMaster.Models;
using QuizMaster.Repositories;
using BCrypt.Net;

namespace QuizMaster.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository, IRoleRepository roleRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<UserResponseDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<UserResponseDto>>(users);
        }

        public async Task<UserResponseDto?> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return null;

            return _mapper.Map<UserResponseDto>(user);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _userRepository.GetByEmailAsync(email);
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _userRepository.GetByUsernameAsync(username);
        }

        public async Task<User> RegisterUserAsync(string firstName, string lastName, string email, string username, string password, int roleId, string? organizationName = null, string? description = null)
        {
            if (await _userRepository.EmailExistsAsync(email))
                throw new ArgumentException("Email already exists");

            if (await _userRepository.UsernameExistsAsync(username))
                throw new ArgumentException("Username already exists");

            if (!await _roleRepository.ExistsAsync(roleId))
                throw new ArgumentException("Role does not exist");

            string salt = BCrypt.Net.BCrypt.GenerateSalt();
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password, salt);

            var user = new User
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Username = username,
                PasswordHash = hashedPassword,
                PasswordSalt = salt,
                RoleId = roleId,
                OrganizationName = organizationName,
                Description = description,
                IsApproved = roleId != 2 

            };

            var createdUser = await _userRepository.CreateAsync(user);
            return await _userRepository.GetByIdAsync(createdUser.Id) ?? createdUser;
        }

        public async Task<User?> LoginAsync(string usernameOrEmail, string password)
        {
            User? user = null;

            if (usernameOrEmail.Contains("@"))
                user = await _userRepository.GetByEmailAsync(usernameOrEmail);
            else
                user = await _userRepository.GetByUsernameAsync(usernameOrEmail);

            if (user == null) return null;

            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                return null;

            return await _userRepository.GetByIdAsync(user.Id);
        }

        public async Task<User> UpdateUserAsync(User user)
        {
            return await _userRepository.UpdateAsync(user);
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            return await _userRepository.DeleteAsync(id);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _userRepository.EmailExistsAsync(email);
        }

        public async Task<bool> UsernameExistsAsync(string username)
        {
            return await _userRepository.UsernameExistsAsync(username);
        }
    }
}