using Microsoft.EntityFrameworkCore;
using QuizMaster.Data;
using QuizMaster.Models;

namespace QuizMaster.Repositories
{
    public class SubscriptionRepository : ISubscriptionRepository
    {
        private readonly ApplicationDbContext _context;

        public SubscriptionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Subscription>> GetAllAsync()
        {
            return await _context.Subscriptions
                .Include(s => s.Subscriber)
                .Include(s => s.Organizer)
                .ToListAsync();
        }

        public async Task<Subscription?> GetByIdAsync(int id)
        {
            return await _context.Subscriptions
                .Include(s => s.Subscriber)
                .Include(s => s.Organizer)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<IEnumerable<Subscription>> GetBySubscriberIdAsync(int subscriberId)
        {
            return await _context.Subscriptions
                .Include(s => s.Organizer)
                .Where(s => s.SubscriberId == subscriberId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Subscription>> GetByOrganizerIdAsync(int organizerId)
        {
            return await _context.Subscriptions
                .Include(s => s.Subscriber)
                .Where(s => s.OrganizerId == organizerId)
                .ToListAsync();
        }

        public async Task<Subscription?> GetBySubscriberAndOrganizerAsync(int subscriberId, int organizerId)
        {
            return await _context.Subscriptions
                .Include(s => s.Subscriber)
                .Include(s => s.Organizer)
                .FirstOrDefaultAsync(s => s.SubscriberId == subscriberId && s.OrganizerId == organizerId);
        }

        public async Task<Subscription> CreateAsync(Subscription subscription)
        {
            _context.Subscriptions.Add(subscription);
            await _context.SaveChangesAsync();

            return await GetByIdAsync(subscription.Id) ?? subscription;
        }

        public async Task<Subscription> UpdateAsync(Subscription subscription)
        {
            _context.Entry(subscription).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return await GetByIdAsync(subscription.Id) ?? subscription;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var subscription = await _context.Subscriptions.FindAsync(id);
            if (subscription == null) return false;

            _context.Subscriptions.Remove(subscription);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Subscriptions.AnyAsync(s => s.Id == id);
        }

        public async Task<bool> IsSubscribedAsync(int subscriberId, int organizerId)
        {
            return await _context.Subscriptions
                .AnyAsync(s => s.SubscriberId == subscriberId && s.OrganizerId == organizerId && s.IsActive);
        }

        public async Task<bool> ToggleSubscriptionAsync(int subscriberId, int organizerId)
        {
            var existing = await GetBySubscriberAndOrganizerAsync(subscriberId, organizerId);

            if (existing != null)
            {
                existing.IsActive = !existing.IsActive;
                await UpdateAsync(existing);
                return existing.IsActive;
            }
            else
            {
                var subscription = new Subscription
                {
                    SubscriberId = subscriberId,
                    OrganizerId = organizerId,
                    IsActive = true
                };
                await CreateAsync(subscription);
                return true;
            }
        }
    }
}
