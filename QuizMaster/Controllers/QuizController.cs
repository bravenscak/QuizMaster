using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuizMaster.DTOs;
using QuizMaster.Services;
using System.Security.Claims;

namespace QuizMaster.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuizController : ControllerBase
    {
        private readonly IQuizService _quizService;

        public QuizController(IQuizService quizService)
        {
            _quizService = quizService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<QuizDto>>> GetQuizzes([FromQuery] QuizSearchDto? searchDto = null)
        {
            searchDto ??= new QuizSearchDto();
            var quizzes = await _quizService.SearchUpcomingQuizzesAsync(searchDto);
            return Ok(quizzes);
        }

        [HttpGet("my")]
        [Authorize(Roles = "ORGANIZER,ADMIN")]
        public async Task<ActionResult<IEnumerable<QuizDto>>> GetMyQuizzes()
        {
            var userId = GetCurrentUserId();
            var quizzes = await _quizService.GetQuizzesByOrganizerAsync(userId);
            return Ok(quizzes);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<QuizDto>> GetQuiz(int id)
        {
            var quiz = await _quizService.GetQuizByIdAsync(id);
            if (quiz == null)
                return NotFound();

            return Ok(quiz);
        }

        [HttpPost]
        [Authorize(Roles = "ORGANIZER")]
        public async Task<ActionResult<QuizDto>> CreateQuiz([FromBody] CreateQuizDto createQuizDto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var quiz = await _quizService.CreateQuizAsync(createQuizDto, userId);
                return CreatedAtAction(nameof(GetQuiz), new { id = quiz.Id }, quiz);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "ORGANIZER,ADMIN")]
        public async Task<ActionResult<QuizDto>> UpdateQuiz(int id, [FromBody] UpdateQuizDto updateQuizDto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var userRole = GetCurrentUserRole();

                if (!await _quizService.CanUserModifyQuizAsync(id, userId, userRole))
                    return Forbid("You can only modify your own quizzes");

                var quiz = await _quizService.UpdateQuizAsync(id, updateQuizDto, userId, userRole);  
                return Ok(quiz);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "ORGANIZER,ADMIN")]
        public async Task<ActionResult> DeleteQuiz(int id)
        {
            var userId = GetCurrentUserId();
            var userRole = GetCurrentUserRole();

            var success = await _quizService.DeleteQuizAsync(id, userId, userRole);
            if (!success)
                return Forbid("You can only delete your own quizzes");

            return NoContent();
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                throw new UnauthorizedAccessException("Invalid user token");

            return userId;
        }

        private string GetCurrentUserRole()
        {
            var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;
            if (string.IsNullOrEmpty(roleClaim))
                throw new UnauthorizedAccessException("Invalid user role");

            return roleClaim;
        }
    }
}
