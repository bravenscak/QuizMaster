using AutoMapper;
using QuizMaster.DTOs;
using QuizMaster.Models;
using QuizMaster.Repositories;

namespace QuizMaster.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IQuizRepository _quizRepository;
        private readonly IMapper _mapper;

        public CategoryService(ICategoryRepository categoryRepository, IQuizRepository quizRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _quizRepository = quizRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
        {
            var categories = await _categoryRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<CategoryDto>>(categories);
        }

        public async Task<CategoryDto?> GetCategoryByIdAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null) return null;

            return _mapper.Map<CategoryDto>(category);
        }

        public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto createCategoryDto)
        {
            if (await _categoryRepository.NameExistsAsync(createCategoryDto.Name))
                throw new ArgumentException($"Category with name '{createCategoryDto.Name}' already exists");

            var category = _mapper.Map<Category>(createCategoryDto);
            var createdCategory = await _categoryRepository.CreateAsync(category);

            return _mapper.Map<CategoryDto>(createdCategory);
        }

        public async Task<CategoryDto> UpdateCategoryAsync(int id, UpdateCategoryDto updateCategoryDto)
        {
            var existingCategory = await _categoryRepository.GetByIdAsync(id);
            if (existingCategory == null)
                throw new ArgumentException("Category not found");

            if (await _categoryRepository.NameExistsAsync(updateCategoryDto.Name, id))
                throw new ArgumentException($"Category with name '{updateCategoryDto.Name}' already exists");

            existingCategory.Name = updateCategoryDto.Name;

            var updatedCategory = await _categoryRepository.UpdateAsync(existingCategory);
            return _mapper.Map<CategoryDto>(updatedCategory);
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var quizzesInCategory = await _quizRepository.GetByCategoryIdAsync(id);
            if (quizzesInCategory.Any())
            {
                throw new ArgumentException($"Ne možete obrisati kategoriju jer sadrži {quizzesInCategory.Count()} kvizova. Prvo premjestite ili obrišite kvizove.");
            }

            return await _categoryRepository.DeleteAsync(id);
        }
    }
}