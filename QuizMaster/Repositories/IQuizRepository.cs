using QuizMaster.Models;

namespace QuizMaster.Repositories
{
    public interface IQuizRepository
    {
        Task<IEnumerable<Quiz>> GetAllAsync();
        Task<Quiz?> GetByIdAsync(int id);
        Task<IEnumerable<Quiz>> GetByOrganizerIdAsync(int organizerId);
        Task<IEnumerable<Quiz>> GetByCategoryIdAsync(int categoryId);
        Task<IEnumerable<Quiz>> GetUpcomingQuizzesAsync();
        Task<IEnumerable<Quiz>> GetPastQuizzesAsync();
        Task<Quiz> CreateAsync(Quiz quiz);
        Task<Quiz> UpdateAsync(Quiz quiz);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<int> GetRegisteredTeamsCountAsync(int quizId);
    }
}
