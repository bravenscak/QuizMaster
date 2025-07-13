using AutoMapper;
using QuizMaster.DTOs;
using QuizMaster.Models;
using QuizMaster.Repositories;

namespace QuizMaster.Services
{
    public class QuizService : IQuizService
    {
        private readonly IQuizRepository _quizRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ITeamRepository _teamRepository;
        private readonly INotificationService _notificationService;
        private readonly IMapper _mapper;

        public QuizService(IQuizRepository quizRepository, ICategoryRepository categoryRepository, ITeamRepository teamRepository, INotificationService notificationService, IMapper mapper)
        {
            _quizRepository = quizRepository;
            _categoryRepository = categoryRepository;
            _teamRepository = teamRepository; 
            _notificationService = notificationService;
            _mapper = mapper;
        }

        public async Task<IEnumerable<QuizDto>> SearchUpcomingQuizzesAsync(QuizSearchDto searchDto)
        {
            var quizzes = await _quizRepository.SearchUpcomingQuizzesAsync(searchDto);
            return await MapQuizzesToDtos(quizzes);
        }

        public async Task<QuizDto?> GetQuizByIdAsync(int id)
        {
            var quiz = await _quizRepository.GetByIdAsync(id);
            if (quiz == null) return null;

            return await MapQuizToDto(quiz);
        }

        public async Task<IEnumerable<QuizDto>> GetQuizzesByOrganizerAsync(int organizerId)
        {
            var quizzes = await _quizRepository.GetByOrganizerIdAsync(organizerId);
            return await MapQuizzesToDtos(quizzes);
        }

        public async Task<QuizDto> CreateQuizAsync(CreateQuizDto createQuizDto, int organizerId)
        {
            if (!await _categoryRepository.ExistsAsync(createQuizDto.CategoryId))
                throw new ArgumentException("Category does not exist");

            if (createQuizDto.DateTime <= DateTime.UtcNow)
                throw new ArgumentException("Quiz date must be in the future");

            var quiz = _mapper.Map<Quiz>(createQuizDto);
            quiz.UserId = organizerId; 

            var createdQuiz = await _quizRepository.CreateAsync(quiz);

            await _notificationService.CreateNewQuizNotificationAsync(createdQuiz.Id, organizerId);

            return await MapQuizToDto(createdQuiz);
        }

        public async Task<QuizDto> UpdateQuizAsync(int id, UpdateQuizDto updateQuizDto, int organizerId, string userRole)
        {
            var existingQuiz = await _quizRepository.GetByIdAsync(id);
            if (existingQuiz == null)
                throw new ArgumentException("Quiz not found");

            if (!await CanUserModifyQuizAsync(id, organizerId, userRole))
                throw new UnauthorizedAccessException("You can only modify your own quizzes");

            if (existingQuiz.DateTime <= DateTime.UtcNow.AddHours(24))
                throw new ArgumentException("Ne možete mijenjati kviz manje od 24 sata prije početka");

            var currentRegisteredCount = await _quizRepository.GetRegisteredTeamsCountAsync(id);
            if (updateQuizDto.MaxTeams < currentRegisteredCount)
                throw new ArgumentException($"Ne možete postaviti maksimum na {updateQuizDto.MaxTeams} timova jer imate {currentRegisteredCount} prijavljenih timova");

            var maxParticipantsInExistingTeams = await _teamRepository.GetMaxParticipantsInTeamsAsync(id);
            if (updateQuizDto.MaxParticipantsPerTeam < maxParticipantsInExistingTeams)
                throw new ArgumentException($"Ne možete postaviti maksimum na {updateQuizDto.MaxParticipantsPerTeam} igrača jer postoji tim s {maxParticipantsInExistingTeams} igrača");

            if (!await _categoryRepository.ExistsAsync(updateQuizDto.CategoryId))
                throw new ArgumentException("Category does not exist");

            existingQuiz.Name = updateQuizDto.Name;
            existingQuiz.LocationName = updateQuizDto.LocationName;
            existingQuiz.Address = updateQuizDto.Address;
            existingQuiz.Latitude = updateQuizDto.Latitude;
            existingQuiz.Longitude = updateQuizDto.Longitude;
            existingQuiz.EntryFee = updateQuizDto.EntryFee;
            existingQuiz.DateTime = updateQuizDto.DateTime;
            existingQuiz.MaxParticipantsPerTeam = updateQuizDto.MaxParticipantsPerTeam;
            existingQuiz.MaxTeams = updateQuizDto.MaxTeams;
            existingQuiz.DurationMinutes = updateQuizDto.DurationMinutes;
            existingQuiz.Description = updateQuizDto.Description;
            existingQuiz.CategoryId = updateQuizDto.CategoryId;

            var updatedQuiz = await _quizRepository.UpdateAsync(existingQuiz);
            return await MapQuizToDto(updatedQuiz);
        }

        public async Task<bool> DeleteQuizAsync(int id, int organizerId, string userRole)
        {
            if (!await CanUserModifyQuizAsync(id, organizerId, userRole))
                return false;

            return await _quizRepository.DeleteAsync(id);
        }

        public async Task<bool> CanUserModifyQuizAsync(int quizId, int userId, string userRole)
        {
            if (userRole == "ADMIN") return true;

            var quiz = await _quizRepository.GetByIdAsync(quizId);
            if (quiz == null) return false;

            return quiz.UserId == userId;
        }

        private async Task<QuizDto> MapQuizToDto(Quiz quiz)
        {
            var quizDto = _mapper.Map<QuizDto>(quiz);

            quizDto.RegisteredTeamsCount = await _quizRepository.GetRegisteredTeamsCountAsync(quiz.Id);

            return quizDto;
        }

        private async Task<IEnumerable<QuizDto>> MapQuizzesToDtos(IEnumerable<Quiz> quizzes)
        {
            var quizDtos = new List<QuizDto>();
            foreach (var quiz in quizzes)
            {
                quizDtos.Add(await MapQuizToDto(quiz));
            }
            return quizDtos;
        }
    }
}