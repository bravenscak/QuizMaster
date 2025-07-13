using QuizMaster.Models;

namespace QuizMaster.Repositories
{
    public interface ITeamRepository
    {
        Task<IEnumerable<Team>> GetAllAsync();
        Task<Team?> GetByIdAsync(int id);
        Task<IEnumerable<Team>> GetByQuizIdAsync(int quizId);
        Task<IEnumerable<Team>> GetByUserIdAsync(int userId);
        Task<Team?> GetByUserAndQuizAsync(int userId, int quizId);
        Task<Team> CreateAsync(Team team);
        Task<Team> UpdateAsync(Team team);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<int> GetTeamCountByQuizAsync(int quizId);
        Task<bool> UserHasTeamInQuizAsync(int userId, int quizId);
        Task<int> GetMaxParticipantsInTeamsAsync(int quizId);
    }
}
