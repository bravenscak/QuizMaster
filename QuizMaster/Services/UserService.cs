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

        public async Task<UserResponseDto?> UpdateUserAsync(int userId, UpdateUserDto updateUserDto)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return null;

            var existingUserWithEmail = await _userRepository.GetByEmailAsync(updateUserDto.Email);
            if (existingUserWithEmail != null && existingUserWithEmail.Id != userId)
                throw new ArgumentException("Email već postoji u sustavu");

            user.FirstName = updateUserDto.FirstName;
            user.LastName = updateUserDto.LastName;
            user.Email = updateUserDto.Email;
            user.OrganizationName = updateUserDto.OrganizationName;
            user.Description = updateUserDto.Description;

            var updatedUser = await _userRepository.UpdateAsync(user);

            return new UserResponseDto
            {
                Id = updatedUser.Id,
                FirstName = updatedUser.FirstName,
                LastName = updatedUser.LastName,
                Email = updatedUser.Email,
                Username = updatedUser.Username,
                OrganizationName = updatedUser.OrganizationName,
                Description = updatedUser.Description,
                RoleName = updatedUser.Role.Name,
                IsApproved = updatedUser.IsApproved
            };
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

        public async Task ChangePasswordAsync(int userId, ChangePasswordDto changePasswordDto)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new ArgumentException("Korisnik nije pronađen");

            var currentHashedPassword = BCrypt.Net.BCrypt.HashPassword(changePasswordDto.CurrentPassword, user.PasswordSalt);
            if (currentHashedPassword != user.PasswordHash)
                throw new UnauthorizedAccessException("Trenutna lozinka nije ispravna");

            var newSalt = BCrypt.Net.BCrypt.GenerateSalt();
            var newHashedPassword = BCrypt.Net.BCrypt.HashPassword(changePasswordDto.NewPassword, newSalt);

            user.PasswordHash = newHashedPassword;
            user.PasswordSalt = newSalt;

            await _userRepository.UpdateAsync(user);
        }
    }
}