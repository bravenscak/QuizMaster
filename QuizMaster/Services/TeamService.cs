using AutoMapper;
using QuizMaster.DTOs;
using QuizMaster.Models;
using QuizMaster.Repositories;

namespace QuizMaster.Services
{
    public class TeamService : ITeamService
    {
        private readonly ITeamRepository _teamRepository;
        private readonly IQuizRepository _quizRepository;
        private readonly INotificationService _notificationService;
        private readonly IMapper _mapper;

        public TeamService(ITeamRepository teamRepository, IQuizRepository quizRepository, INotificationService notificationService, IMapper mapper)
        {
            _teamRepository = teamRepository;
            _quizRepository = quizRepository;
            _notificationService = notificationService;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TeamDto>> GetAllTeamsAsync()
        {
            var teams = await _teamRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<TeamDto>>(teams);
        }

        public async Task<TeamDto?> GetTeamByIdAsync(int id)
        {
            var team = await _teamRepository.GetByIdAsync(id);
            if (team == null) return null;

            return _mapper.Map<TeamDto>(team);
        }

        public async Task<IEnumerable<QuizTeamDto>> GetTeamsByQuizIdAsync(int quizId)
        {
            var teams = await _teamRepository.GetByQuizIdAsync(quizId);
            return _mapper.Map<IEnumerable<QuizTeamDto>>(teams);
        }

        public async Task<IEnumerable<TeamDto>> GetTeamsByUserIdAsync(int userId)
        {
            var teams = await _teamRepository.GetByUserIdAsync(userId);
            return _mapper.Map<IEnumerable<TeamDto>>(teams);
        }

        public async Task<TeamDto> RegisterTeamAsync(CreateTeamDto createTeamDto, int userId)
        {
            var quiz = await _quizRepository.GetByIdAsync(createTeamDto.QuizId);
            if (quiz == null)
                throw new ArgumentException("Quiz not found");

            if (quiz.DateTime <= DateTime.UtcNow)
                throw new ArgumentException("Cannot register for a quiz that has already started");

            if (await _teamRepository.UserHasTeamInQuizAsync(userId, createTeamDto.QuizId))
                throw new ArgumentException("You already have a team registered for this quiz");

            if (createTeamDto.ParticipantCount > quiz.MaxParticipantsPerTeam)
                throw new ArgumentException($"Team size cannot exceed {quiz.MaxParticipantsPerTeam} participants");

            var currentTeamsCount = await _teamRepository.GetTeamCountByQuizAsync(createTeamDto.QuizId);
            if (currentTeamsCount >= quiz.MaxTeams)
                throw new ArgumentException("Quiz is full, no more teams can register");

            var team = _mapper.Map<Team>(createTeamDto);
            team.UserId = userId; 

            var createdTeam = await _teamRepository.CreateAsync(team);

            await _notificationService.CreateTeamRegisteredNotificationAsync(createdTeam.Id, createTeamDto.QuizId);

            return _mapper.Map<TeamDto>(createdTeam);
        }

        public async Task<TeamDto> UpdateTeamAsync(int id, UpdateTeamDto updateTeamDto, int userId)
        {
            var existingTeam = await _teamRepository.GetByIdAsync(id);
            if (existingTeam == null)
                throw new ArgumentException("Team not found");

            if (!await CanUserModifyTeamAsync(id, userId))
                throw new UnauthorizedAccessException("You can only modify your own teams");

            if (existingTeam.Quiz.DateTime <= DateTime.UtcNow)
                throw new ArgumentException("Cannot modify team for a quiz that has already started");

            if (updateTeamDto.ParticipantCount > existingTeam.Quiz.MaxParticipantsPerTeam)
                throw new ArgumentException($"Team size cannot exceed {existingTeam.Quiz.MaxParticipantsPerTeam} participants");

            existingTeam.Name = updateTeamDto.Name;
            existingTeam.ParticipantCount = updateTeamDto.ParticipantCount;

            var updatedTeam = await _teamRepository.UpdateAsync(existingTeam);
            return _mapper.Map<TeamDto>(updatedTeam);
        }

        public async Task<bool> DeleteTeamAsync(int id, int userId)
        {
            var team = await _teamRepository.GetByIdAsync(id);
            if (team == null) return false;

            if (!await CanUserModifyTeamAsync(id, userId))
                return false;

            if (team.Quiz.DateTime <= DateTime.UtcNow)
                throw new ArgumentException("Cannot delete team for a quiz that has already started");

            return await _teamRepository.DeleteAsync(id);
        }

        public async Task<TeamDto> SetTeamResultAsync(int teamId, TeamResultDto resultDto, int organizerId)
        {
            var team = await _teamRepository.GetByIdAsync(teamId);
            if (team == null)
                throw new ArgumentException("Team not found");

            if (!await CanOrganizerSetResultAsync(teamId, organizerId))
                throw new UnauthorizedAccessException("You can only set results for teams in your quizzes");

            if (team.Quiz.DateTime > DateTime.UtcNow)
                throw new ArgumentException("Cannot set results for a quiz that hasn't finished yet");

            team.FinalPosition = resultDto.FinalPosition;

            var updatedTeam = await _teamRepository.UpdateAsync(team);

            var allTeamsWithResults = await _teamRepository.GetByQuizIdAsync(team.QuizId);
            var teamsWithoutResults = allTeamsWithResults.Where(t => !t.FinalPosition.HasValue).Count();

            if (teamsWithoutResults == 0) 
            {
                await _notificationService.CreateQuizResultsNotificationAsync(team.QuizId);
            }

            return _mapper.Map<TeamDto>(updatedTeam);
        }

        public async Task<bool> CanUserModifyTeamAsync(int teamId, int userId)
        {
            var team = await _teamRepository.GetByIdAsync(teamId);
            if (team == null) return false;

            return team.UserId == userId;
        }

        public async Task<bool> CanOrganizerSetResultAsync(int teamId, int organizerId)
        {
            var team = await _teamRepository.GetByIdAsync(teamId);
            if (team == null) return false;

            return team.Quiz.UserId == organizerId;
        }
    }
}