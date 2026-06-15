using ToDoApp.Domain.Entities;

namespace ToDoApp.Infrastructure.Repositories.Interfaces
{
    public interface ITaskRepository
    {
        Task<TodoTask?> GetByIdAsync(int id, int userId);
        Task<TodoTask> CreateAsync(TodoTask task);
    }
}
