using Microsoft.EntityFrameworkCore;
using QuizMaster.Data;
using QuizMaster.DTOs;
using QuizMaster.Models;
using System.Linq;

namespace QuizMaster.Repositories
{
    public class QuizRepository : IQuizRepository
    {
        private readonly ApplicationDbContext _context;

        public QuizRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Quiz>> SearchUpcomingQuizzesAsync(QuizSearchDto searchDto)
        {
            var query = _context.Quizzes
                .Include(q => q.User)
                .Include(q => q.Category)
                .Where(q => q.DateTime > DateTime.UtcNow) 
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchDto.SearchTerm))
            {
                var searchTerm = searchDto.SearchTerm.ToLower();
                query = query.Where(q =>
                    q.Name.ToLower().Contains(searchTerm) ||
                    q.LocationName.ToLower().Contains(searchTerm) ||
                    q.Address.ToLower().Contains(searchTerm) ||
                    (q.User.FirstName + " " + q.User.LastName).ToLower().Contains(searchTerm) ||
                    (q.User.OrganizationName != null && q.User.OrganizationName.ToLower().Contains(searchTerm))
                );
            }

            if (searchDto.CategoryId.HasValue)
            {
                query = query.Where(q => q.CategoryId == searchDto.CategoryId.Value);
            }

            if (searchDto.OrganizerId.HasValue)
            {
                query = query.Where(q => q.UserId == searchDto.OrganizerId.Value);
            }

            if (searchDto.DateFrom.HasValue)
            {
                query = query.Where(q => q.DateTime >= searchDto.DateFrom.Value);
            }

            if (searchDto.DateTo.HasValue)
            {
                query = query.Where(q => q.DateTime <= searchDto.DateTo.Value);
            }

            if (searchDto.SortBy.HasValue)
            {
                switch (searchDto.SortBy.Value)
                {
                    case QuizSortBy.DateTime:
                        query = searchDto.SortDirection == SortDirection.Ascending
                            ? query.OrderBy(q => q.DateTime)
                            : query.OrderByDescending(q => q.DateTime);
                        break;
                    case QuizSortBy.Name:
                        query = searchDto.SortDirection == SortDirection.Ascending
                            ? query.OrderBy(q => q.Name)
                            : query.OrderByDescending(q => q.Name);
                        break;
                    case QuizSortBy.CategoryName:
                        query = searchDto.SortDirection == SortDirection.Ascending
                            ? query.OrderBy(q => q.Category.Name)
                            : query.OrderByDescending(q => q.Category.Name);
                        break;
                    case QuizSortBy.RegisteredTeams:
                        query = searchDto.SortDirection == SortDirection.Ascending
                            ? query.OrderBy(q => q.Teams.Count)
                            : query.OrderByDescending(q => q.Teams.Count);
                        break;
                }
            }
            else
            {
                query = query.OrderBy(q => q.DateTime);
            }

            return await query.ToListAsync();
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
