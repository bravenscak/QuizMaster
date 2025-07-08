using QuizMaster.Models;

namespace QuizMaster.Repositories
{
    public interface ISubscriptionRepository
    {
        Task<IEnumerable<Subscription>> GetAllAsync();
        Task<Subscription?> GetByIdAsync(int id);
        Task<IEnumerable<Subscription>> GetBySubscriberIdAsync(int subscriberId);
        Task<IEnumerable<Subscription>> GetByOrganizerIdAsync(int organizerId);
        Task<Subscription?> GetBySubscriberAndOrganizerAsync(int subscriberId, int organizerId);
        Task<Subscription> CreateAsync(Subscription subscription);
        Task<Subscription> UpdateAsync(Subscription subscription);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<bool> IsSubscribedAsync(int subscriberId, int organizerId);
        Task<bool> ToggleSubscriptionAsync(int subscriberId, int organizerId);
    }
}
