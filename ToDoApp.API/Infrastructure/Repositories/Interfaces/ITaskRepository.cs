using ToDoApp.API.Core.DTOs.Pagination;
using ToDoApp.Core.DTOs.Task;
using ToDoApp.Domain.Entities;

namespace ToDoApp.Infrastructure.Repositories.Interfaces
{
    public interface ITaskRepository
    {
        Task<TodoTask?> GetByIdAsync(int id, int userId);
        Task<TodoTask> CreateAsync(TodoTask task);
        Task<(IEnumerable<TodoTask> Items, int TotalCount)> GetTasksByUserIdPagedAsync(int userId, int pageNumber, int pageSize, int? categoryId);
        Task<TodoTask?> UpdateAsync(TodoTask task);
        Task<bool> DeleteAsync(TodoTask task);
    }
}
