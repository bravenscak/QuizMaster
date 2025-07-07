using QuizMaster.DTOs;

namespace QuizMaster.Services
{
    public interface IQuizService
    {
        Task<IEnumerable<QuizDto>> GetAllQuizzesAsync();
        Task<QuizDto?> GetQuizByIdAsync(int id);
        Task<IEnumerable<QuizDto>> GetQuizzesByOrganizerAsync(int organizerId);
        Task<IEnumerable<QuizDto>> GetQuizzesByCategoryAsync(int categoryId);
        Task<IEnumerable<QuizDto>> GetUpcomingQuizzesAsync();
        Task<IEnumerable<QuizDto>> GetPastQuizzesAsync();
        Task<QuizDto> CreateQuizAsync(CreateQuizDto createQuizDto, int organizerId);
        Task<QuizDto> UpdateQuizAsync(int id, UpdateQuizDto updateQuizDto, int organizerId, string userRole);
        Task<bool> DeleteQuizAsync(int id, int organizerId, string userRole);
        Task<bool> CanUserModifyQuizAsync(int quizId, int userId, string userRole);
    }
}
