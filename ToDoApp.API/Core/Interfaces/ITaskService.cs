using ToDoApp.API.Core.DTOs.Task;
using ToDoApp.Core.DTOs.Task;

namespace ToDoApp.Core.Interfaces
{
    public interface ITaskService
    {
        Task<TaskResponseDto?> GetByIdAsync(int id, int userId);
        Task<TaskResponseDto> CreateTaskAsync(CreateTaskDto dto, int userId);
        Task<IEnumerable<TaskResponseDto>> GetUserTasksAsync(int userId);
        Task<TaskResponseDto?> UpdateTaskAsync(int id, UpdateTaskDto dto, int userId);
        Task<bool> DeleteTaskAsync(int id, int userId);
    }
}
