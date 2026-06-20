using ToDoApp.Core.DTOs.Category;
using ToDoApp.Core.Interfaces;
using ToDoApp.Domain.Entities;
using ToDoApp.Infrastructure.Repositories.Interfaces;

namespace ToDoApp.Core.Services
{
    public class CategoryService(ICategoryRepository categoryRepository) : ICategoryService
    {
        public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto dto, int userId)
        {
            var normalizedName = dto.Name.Trim();

            var existingCategories = await categoryRepository.GetByUserIdAsync(userId);

            if (existingCategories.Any(c => c.Name.Equals(normalizedName, StringComparison.OrdinalIgnoreCase)))
            {
                throw new Exception($"Category with the name '{normalizedName}' already exists.");
            }

            var category = new Category
            {
                Name = normalizedName,
                UserId = userId
            };

            var created = await categoryRepository.CreateAsync(category);

            return new CategoryDto
            {
                Id = created.Id,
                Name = created.Name
            };
        }

        public async Task<IEnumerable<CategoryDto>> GetUserCategoriesAsync(int userId)
        {
            var categories = await categoryRepository.GetByUserIdAsync(userId);

            return categories.Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name
            });
        }
    }
}
