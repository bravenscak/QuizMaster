using QuizMaster.DTOs;

namespace QuizMaster.Services
{
    public interface ITeamService
    {
        Task<IEnumerable<TeamDto>> GetAllTeamsAsync();
        Task<TeamDto?> GetTeamByIdAsync(int id);
        Task<IEnumerable<QuizTeamDto>> GetTeamsByQuizIdAsync(int quizId);
        Task<IEnumerable<TeamDto>> GetTeamsByUserIdAsync(int userId);
        Task<TeamDto> RegisterTeamAsync(CreateTeamDto createTeamDto, int userId);
        Task<TeamDto> UpdateTeamAsync(int id, UpdateTeamDto updateTeamDto, int userId);
        Task<bool> DeleteTeamAsync(int id, int userId);
        Task<TeamDto> SetTeamResultAsync(int teamId, TeamResultDto resultDto, int organizerId);
        Task<bool> CanUserModifyTeamAsync(int teamId, int userId);
        Task<bool> CanOrganizerSetResultAsync(int teamId, int organizerId);
    }
}
