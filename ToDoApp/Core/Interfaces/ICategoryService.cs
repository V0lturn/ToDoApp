using ToDoApp.Core.DTOs.Category;

namespace ToDoApp.Core.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> GetUserCategoriesAsync(int userId);
        Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto dto, int userId);
    }
}
