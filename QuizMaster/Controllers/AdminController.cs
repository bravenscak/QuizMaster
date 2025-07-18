﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizMaster.DTOs;
using QuizMaster.Services;
using QuizMaster.Repositories;

namespace QuizMaster.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "ADMIN")] 
    public class AdminController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IQuizService _quizService;
        private readonly IUserRepository _userRepository;

        public AdminController(IUserService userService, IQuizService quizService, IUserRepository userRepository)
        {
            _userService = userService;
            _quizService = quizService;
            _userRepository = userRepository;
        }

        [HttpGet("pending-organizers")]
        public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetPendingOrganizers()
        {
            var pendingUsers = await _userRepository.GetPendingOrganizersAsync();
            return Ok(pendingUsers.Select(u => new UserResponseDto
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                Username = u.Username,
                OrganizationName = u.OrganizationName,
                Description = u.Description,
                RoleName = u.Role.Name,
                IsApproved = u.IsApproved
            }));
        }

        [HttpPut("approve-organizer/{id}")]
        public async Task<ActionResult> ApproveOrganizer(int id)
        {
            var success = await _userRepository.ApproveOrganizerAsync(id);

            if (!success)
                return NotFound("Organizer not found or already approved");

            return Ok(new { message = "Organizer approved successfully" });
        }

        [HttpPut("reject-organizer/{id}")]
        public async Task<ActionResult> RejectOrganizer(int id)
        {
            var success = await _userService.DeleteUserAsync(id);

            if (!success)
                return NotFound("Organizer not found");

            return Ok(new { message = "Organizer rejected and deleted" });
        }

        [HttpGet("users")]
        public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpDelete("users/{id}")]
        public async Task<ActionResult> DeleteUser(int id)
        {
            var success = await _userService.DeleteUserAsync(id);

            if (!success)
                return NotFound();

            return NoContent();
        }

        [HttpGet("all-quizzes")]
        public async Task<ActionResult<IEnumerable<QuizDto>>> GetAllQuizzes()
        {
            var users = await _userService.GetAllUsersAsync();
            var organizers = users.Where(u => u.RoleName == "ORGANIZER");

            var allQuizzes = new List<QuizDto>();
            foreach (var organizer in organizers)
            {
                var quizzes = await _quizService.GetQuizzesByOrganizerAsync(organizer.Id);
                allQuizzes.AddRange(quizzes);
            }

            return Ok(allQuizzes.OrderByDescending(q => q.DateTime));
        }
    }
}