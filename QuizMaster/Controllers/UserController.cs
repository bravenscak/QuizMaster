using Microsoft.AspNetCore.Mvc;
using QuizMaster.Services;
using QuizMaster.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace QuizMaster.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetUsers()
        {
            var users = await _userService.GetAllUsersAsync();

            var userDtos = users.Select(u => new UserResponseDto
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                Username = u.Username,
                OrganizationName = u.OrganizationName,
                Description = u.Description,
                RoleName = u.RoleName
            });

            return Ok(userDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserResponseDto>> GetUser(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound();

            var userDto = new UserResponseDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Username = user.Username,
                OrganizationName = user.OrganizationName,
                Description = user.Description,
                RoleName = user.RoleName
            };

            return Ok(userDto);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser(int id)
        {
            var success = await _userService.DeleteUserAsync(id);
            if (!success)
                return NotFound();

            return NoContent();
        }

        [HttpPut("profile")]
        public async Task<ActionResult<UserResponseDto>> UpdateProfile([FromBody] UpdateUserDto updateUserDto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var updatedUser = await _userService.UpdateUserAsync(userId, updateUserDto);

                if (updatedUser == null)
                    return NotFound();

                var userDto = new UserResponseDto
                {
                    Id = updatedUser.Id,
                    FirstName = updatedUser.FirstName,
                    LastName = updatedUser.LastName,
                    Email = updatedUser.Email,
                    Username = updatedUser.Username,
                    OrganizationName = updatedUser.OrganizationName,
                    Description = updatedUser.Description,
                    RoleName = updatedUser.RoleName,
                    IsApproved = updatedUser.IsApproved
                };

                return Ok(userDto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("change-password")]
        public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            try
            {
                var userId = GetCurrentUserId();
                await _userService.ChangePasswordAsync(userId, changePasswordDto);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                throw new UnauthorizedAccessException("Invalid user token");
            return userId;
        }
    }
}