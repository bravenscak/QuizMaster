using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuizMaster.DTOs;
using QuizMaster.Services;

namespace QuizMaster.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeamController : ControllerBase
    {
        private readonly ITeamService _teamService;

        public TeamController(ITeamService teamService)
        {
            _teamService = teamService;
        }

        [HttpGet("quiz/{quizId}")]
        public async Task<ActionResult<IEnumerable<QuizTeamDto>>> GetTeamsByQuiz(int quizId)
        {
            var teams = await _teamService.GetTeamsByQuizIdAsync(quizId);
            return Ok(teams);
        }

        [HttpGet("my")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<TeamDto>>> GetMyTeams()
        {
            var userId = GetCurrentUserId();
            var teams = await _teamService.GetTeamsByUserIdAsync(userId);
            return Ok(teams);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TeamDto>> GetTeam(int id)
        {
            var team = await _teamService.GetTeamByIdAsync(id);
            if (team == null)
                return NotFound();

            return Ok(team);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<TeamDto>> RegisterTeam([FromBody] CreateTeamDto createTeamDto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var team = await _teamService.RegisterTeamAsync(createTeamDto, userId);
                return CreatedAtAction(nameof(GetTeam), new { id = team.Id }, team);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<TeamDto>> UpdateTeam(int id, [FromBody] UpdateTeamDto updateTeamDto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var team = await _teamService.UpdateTeamAsync(id, updateTeamDto, userId);
                return Ok(team);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult> DeleteTeam(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var success = await _teamService.DeleteTeamAsync(id, userId);
                if (!success)
                    return NotFound();

                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}/result")]
        [Authorize(Roles = "ORGANIZER")]
        public async Task<ActionResult<TeamDto>> SetTeamResult(int id, [FromBody] TeamResultDto resultDto)
        {
            try
            {
                var organizerId = GetCurrentUserId();
                var team = await _teamService.SetTeamResultAsync(id, resultDto, organizerId);
                return Ok(team);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, ex.Message);
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
