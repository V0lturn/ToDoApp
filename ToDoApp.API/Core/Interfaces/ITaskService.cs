using ToDoApp.Core.DTOs.Pagination;
using ToDoApp.Core.DTOs.Task;

namespace ToDoApp.Core.Interfaces
{
    public interface ITaskService
    {
        Task<TaskResponseDto?> GetByIdAsync(int id, int userId);
        Task<TaskResponseDto> CreateTaskAsync(CreateTaskDto dto, int userId);
        Task<PagedResponseDto<TaskResponseDto>> GetUserTasksPagedAsync(
            int userId, int pageNumber, int pageSize, int? categoryId, string? searchTerm);
        Task<TaskResponseDto?> UpdateTaskAsync(int id, UpdateTaskDto dto, int userId);
        Task<bool> DeleteTaskAsync(int id, int userId);
    }
}
