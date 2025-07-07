using QuizMaster.DTOs;
using QuizMaster.Models;
using QuizMaster.Repositories;

namespace QuizMaster.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
        {
            var categories = await _categoryRepository.GetAllAsync();
            return categories.Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name
            });
        }

        public async Task<CategoryDto?> GetCategoryByIdAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null) return null;

            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name
            };
        }

        public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto createCategoryDto)
        {
            if (await _categoryRepository.NameExistsAsync(createCategoryDto.Name))
                throw new ArgumentException($"Category with name '{createCategoryDto.Name}' already exists");

            var category = new Category
            {
                Name = createCategoryDto.Name
            };

            var createdCategory = await _categoryRepository.CreateAsync(category);

            return new CategoryDto
            {
                Id = createdCategory.Id,
                Name = createdCategory.Name
            };
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

            return new CategoryDto
            {
                Id = updatedCategory.Id,
                Name = updatedCategory.Name
            };
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            return await _categoryRepository.DeleteAsync(id);
        }
    }

}
