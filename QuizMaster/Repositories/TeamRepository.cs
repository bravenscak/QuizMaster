using Microsoft.EntityFrameworkCore;
using QuizMaster.Data;
using QuizMaster.Models;

namespace QuizMaster.Repositories
{
    public class TeamRepository : ITeamRepository
    {
        private readonly ApplicationDbContext _context;

        public TeamRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Team>> GetAllAsync()
        {
            return await _context.Teams
                .Include(t => t.User)
                .Include(t => t.Quiz)
                .ToListAsync();
        }

        public async Task<Team?> GetByIdAsync(int id)
        {
            return await _context.Teams
                .Include(t => t.User)
                .Include(t => t.Quiz)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<IEnumerable<Team>> GetByQuizIdAsync(int quizId)
        {
            return await _context.Teams
                .Include(t => t.User)
                .Include(t => t.Quiz)
                .Where(t => t.QuizId == quizId)
                .OrderBy(t => t.FinalPosition ?? int.MaxValue)
                .ThenBy(t => t.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Team>> GetByUserIdAsync(int userId)
        {
            return await _context.Teams
                .Include(t => t.User)
                .Include(t => t.Quiz)
                    .ThenInclude(q => q.Category)
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.Quiz.DateTime)
                .ToListAsync();
        }

        public async Task<Team?> GetByUserAndQuizAsync(int userId, int quizId)
        {
            return await _context.Teams
                .Include(t => t.User)
                .Include(t => t.Quiz)
                .FirstOrDefaultAsync(t => t.UserId == userId && t.QuizId == quizId);
        }

        public async Task<Team> CreateAsync(Team team)
        {
            _context.Teams.Add(team);
            await _context.SaveChangesAsync();

            return await GetByIdAsync(team.Id) ?? team;
        }

        public async Task<Team> UpdateAsync(Team team)
        {
            _context.Entry(team).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return await GetByIdAsync(team.Id) ?? team;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var team = await _context.Teams.FindAsync(id);
            if (team == null) return false;

            _context.Teams.Remove(team);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Teams.AnyAsync(t => t.Id == id);
        }

        public async Task<int> GetTeamCountByQuizAsync(int quizId)
        {
            return await _context.Teams.CountAsync(t => t.QuizId == quizId);
        }

        public async Task<bool> UserHasTeamInQuizAsync(int userId, int quizId)
        {
            return await _context.Teams.AnyAsync(t => t.UserId == userId && t.QuizId == quizId);
        }

        public async Task<int> GetMaxParticipantsInTeamsAsync(int quizId)
        {
            var teams = await _context.Teams
                .Where(t => t.QuizId == quizId)
                .ToListAsync();

            return teams.Any() ? teams.Max(t => t.ParticipantCount) : 0;
        }
    }
}
