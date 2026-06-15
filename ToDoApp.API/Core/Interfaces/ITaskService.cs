using ToDoApp.Core.DTOs.Task;

namespace ToDoApp.Core.Interfaces
{
    public interface ITaskService
    {
        Task<TaskResponseDto?> GetByIdAsync(int id, int userId);
        Task<TaskResponseDto> CreateTaskAsync(CreateTaskDto dto, int userId);

    }
}
