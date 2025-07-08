using QuizMaster.DTOs;
using QuizMaster.Models;

namespace QuizMaster.Repositories
{
    public interface IQuizRepository
    {
        Task<Quiz?> GetByIdAsync(int id);
        Task<IEnumerable<Quiz>> GetByOrganizerIdAsync(int organizerId);
        Task<IEnumerable<Quiz>> SearchUpcomingQuizzesAsync(QuizSearchDto searchDto);
        Task<Quiz> CreateAsync(Quiz quiz);
        Task<Quiz> UpdateAsync(Quiz quiz);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<int> GetRegisteredTeamsCountAsync(int quizId);
    }
}
