using Microsoft.EntityFrameworkCore;
using QuizMaster.Data;
using QuizMaster.Models;

namespace QuizMaster.Repositories
{
    public class QuizRepository : IQuizRepository
    {
        private readonly ApplicationDbContext _context;

        public QuizRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Quiz>> GetAllAsync()
        {
            return await _context.Quizzes
                .Include(q => q.User)
                .Include(q => q.Category)
                .OrderByDescending(q => q.DateTime)
                .ToListAsync();
        }

        public async Task<Quiz?> GetByIdAsync(int id)
        {
            return await _context.Quizzes
                .Include(q => q.User)
                .Include(q => q.Category)
                .Include(q => q.Teams)
                .FirstOrDefaultAsync(q => q.Id == id);
        }

        public async Task<IEnumerable<Quiz>> GetByOrganizerIdAsync(int organizerId)
        {
            return await _context.Quizzes
                .Include(q => q.User)        
                .Include(q => q.Category)   
                .Include(q => q.Category)
                .Include(q => q.Teams)
                .Where(q => q.UserId == organizerId)
                .OrderByDescending(q => q.DateTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Quiz>> GetByCategoryIdAsync(int categoryId)
        {
            return await _context.Quizzes
                .Include(q => q.User)
                .Include(q => q.Category)
                .Where(q => q.CategoryId == categoryId)
                .OrderByDescending(q => q.DateTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Quiz>> GetUpcomingQuizzesAsync()
        {
            return await _context.Quizzes
                .Include(q => q.User)
                .Include(q => q.Category)
                .Where(q => q.DateTime > DateTime.UtcNow)
                .OrderBy(q => q.DateTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Quiz>> GetPastQuizzesAsync()
        {
            return await _context.Quizzes
                .Include(q => q.User)
                .Include(q => q.Category)
                .Where(q => q.DateTime <= DateTime.UtcNow)
                .OrderByDescending(q => q.DateTime)
                .ToListAsync();
        }

        public async Task<Quiz> CreateAsync(Quiz quiz)
        {
            _context.Quizzes.Add(quiz);
            await _context.SaveChangesAsync();

            return await GetByIdAsync(quiz.Id) ?? quiz;
        }

        public async Task<Quiz> UpdateAsync(Quiz quiz)
        {
            _context.Entry(quiz).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return await GetByIdAsync(quiz.Id) ?? quiz;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var quiz = await _context.Quizzes.FindAsync(id);
            if (quiz == null) return false;

            _context.Quizzes.Remove(quiz);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Quizzes.AnyAsync(q => q.Id == id);
        }

        public async Task<int> GetRegisteredTeamsCountAsync(int quizId)
        {
            return await _context.Teams.CountAsync(t => t.QuizId == quizId);
        }
    }
}
