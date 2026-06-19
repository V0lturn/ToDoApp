using ToDoApp.Domain.Entities;

namespace ToDoApp.API.Infrastructure.Repositories.Interfaces
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetByUserIdAsync(int userId);
        Task<Category> CreateAsync(Category category);
    }
}
