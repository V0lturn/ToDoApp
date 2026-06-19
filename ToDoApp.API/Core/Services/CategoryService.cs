using ToDoApp.API.Core.DTOs.Category;
using ToDoApp.API.Core.Interfaces;
using ToDoApp.API.Infrastructure.Repositories.Interfaces;
using ToDoApp.Domain.Entities;
using ToDoApp.Infrastructure.Repositories.Interfaces;

namespace ToDoApp.API.Core.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto dto, int userId)
        {
            var category = new Category { Name = dto.Name, UserId = userId };
            var created = await _categoryRepository.CreateAsync(category);
            return new CategoryDto { Id = created.Id, Name = created.Name };
        }

        public async Task<IEnumerable<CategoryDto>> GetUserCategoriesAsync(int userId)
        {
            var categories = await _categoryRepository.GetByUserIdAsync(userId);
            return categories.Select(c => new CategoryDto { Id = c.Id, Name = c.Name });
        }
    }
}
