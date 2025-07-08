using QuizMaster.DTOs;

namespace QuizMaster.Services
{
    public interface INotificationService
    {
        Task<IEnumerable<NotificationDto>> GetUserNotificationsAsync(int userId);
        Task<IEnumerable<NotificationDto>> GetUnreadNotificationsAsync(int userId);
        Task<int> GetUnreadCountAsync(int userId);
        Task<NotificationDto?> GetNotificationByIdAsync(int id, int userId);
        Task<bool> MarkAsReadAsync(int id, int userId);
        Task<bool> MarkAllAsReadAsync(int userId);
        Task<bool> DeleteNotificationAsync(int id, int userId);
        Task CreateNewQuizNotificationAsync(int quizId, int organizerId);
        Task CreateTeamRegisteredNotificationAsync(int teamId, int quizId);
        Task CreateQuizResultsNotificationAsync(int quizId);
    }
}
