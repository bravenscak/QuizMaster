using AutoMapper;
using QuizMaster.DTOs;
using QuizMaster.Models;
using QuizMaster.Repositories;

namespace QuizMaster.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IQuizRepository _quizRepository;
        private readonly ITeamRepository _teamRepository;
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly IMapper _mapper;

        public NotificationService(
            INotificationRepository notificationRepository,
            IQuizRepository quizRepository,
            ITeamRepository teamRepository,
            ISubscriptionRepository subscriptionRepository,
            IMapper mapper)
        {
            _notificationRepository = notificationRepository;
            _quizRepository = quizRepository;
            _teamRepository = teamRepository;
            _subscriptionRepository = subscriptionRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<NotificationDto>> GetUserNotificationsAsync(int userId)
        {
            var notifications = await _notificationRepository.GetByUserIdAsync(userId);
            return _mapper.Map<IEnumerable<NotificationDto>>(notifications);
        }

        public async Task<IEnumerable<NotificationDto>> GetUnreadNotificationsAsync(int userId)
        {
            var notifications = await _notificationRepository.GetUnreadByUserIdAsync(userId);
            return _mapper.Map<IEnumerable<NotificationDto>>(notifications);
        }

        public async Task<int> GetUnreadCountAsync(int userId)
        {
            return await _notificationRepository.GetUnreadCountByUserIdAsync(userId);
        }

        public async Task<NotificationDto?> GetNotificationByIdAsync(int id, int userId)
        {
            var notification = await _notificationRepository.GetByIdAsync(id);
            if (notification == null || notification.UserId != userId)
                return null;

            return _mapper.Map<NotificationDto>(notification);
        }

        private async Task<NotificationDto> CreateNotificationAsync(CreateNotificationDto createNotificationDto)
        {
            var notification = _mapper.Map<Notification>(createNotificationDto);
            notification.CreatedAt = DateTime.UtcNow;

            var createdNotification = await _notificationRepository.CreateAsync(notification);
            return _mapper.Map<NotificationDto>(createdNotification);
        }

        public async Task<bool> MarkAsReadAsync(int id, int userId)
        {
            return await _notificationRepository.MarkAsReadAsync(id, userId);
        }

        public async Task<bool> MarkAllAsReadAsync(int userId)
        {
            return await _notificationRepository.MarkAllAsReadAsync(userId);
        }

        public async Task<bool> DeleteNotificationAsync(int id, int userId)
        {
            var notification = await _notificationRepository.GetByIdAsync(id);
            if (notification == null || notification.UserId != userId)
                return false;

            return await _notificationRepository.DeleteAsync(id);
        }

        public async Task CreateNewQuizNotificationAsync(int quizId, int organizerId)
        {
            var quiz = await _quizRepository.GetByIdAsync(quizId);
            if (quiz == null) return;

            var subscriptions = await _subscriptionRepository.GetByOrganizerIdAsync(organizerId);

            foreach (var subscription in subscriptions.Where(s => s.IsActive))
            {
                var notification = new CreateNotificationDto
                {
                    Title = "Novi kviz!",
                    Message = $"Organizator {quiz.User.FirstName} {quiz.User.LastName} je objavio novi kviz: {quiz.Name}",
                    Type = NotificationTypes.NewQuiz,
                    UserId = subscription.SubscriberId,
                    RelatedEntityId = quizId
                };

                await CreateNotificationAsync(notification);
            }
        }

        public async Task CreateTeamRegisteredNotificationAsync(int teamId, int quizId)
        {
            var team = await _teamRepository.GetByIdAsync(teamId);
            var quiz = await _quizRepository.GetByIdAsync(quizId);

            if (team == null || quiz == null) return;

            var notification = new CreateNotificationDto
            {
                Title = "Nova registracija!",
                Message = $"Tim '{team.Name}' se registrirao na vaš kviz '{quiz.Name}'",
                Type = NotificationTypes.TeamRegistered,
                UserId = quiz.UserId,
                RelatedEntityId = quizId
            };

            await CreateNotificationAsync(notification);
        }

        public async Task CreateQuizResultsNotificationAsync(int quizId)
        {
            var quiz = await _quizRepository.GetByIdAsync(quizId);
            if (quiz == null) return;

            var teams = await _teamRepository.GetByQuizIdAsync(quizId);

            foreach (var team in teams)
            {
                var notification = new CreateNotificationDto
                {
                    Title = "Objavljeni rezultati!",
                    Message = $"Rezultati kviza '{quiz.Name}' su objavljeni",
                    Type = NotificationTypes.QuizResults,
                    UserId = team.UserId,
                    RelatedEntityId = quizId
                };

                await CreateNotificationAsync(notification);
            }
        }
    }
}