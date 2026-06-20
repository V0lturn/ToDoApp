using ToDoApp.Core.DTOs.Pagination;
using ToDoApp.Core.DTOs.Task;
using ToDoApp.Core.Interfaces;
using ToDoApp.Domain.Entities;
using ToDoApp.Infrastructure.Repositories.Interfaces;

namespace ToDoApp.Core.Services
{
    public class TaskService(ITaskRepository taskRepository) : ITaskService
    {
        public async Task<TaskResponseDto> CreateTaskAsync(CreateTaskDto dto, int userId)
        {
            var task = new TodoTask
            {
                Title = dto.Title.Trim(),
                Description = dto.Description ?? string.Empty,
                DueDate = dto.DueDate,
                CategoryId = dto.CategoryId,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            var created = await taskRepository.CreateAsync(task);

            return MapToResponseDto(created);
        }

        public async Task<TaskResponseDto?> GetByIdAsync(int id, int userId)
        {
            var task = await taskRepository.GetByIdAsync(id, userId);

            return task is null ? null : MapToResponseDto(task);
        }

        public async Task<PagedResponseDto<TaskResponseDto>> GetUserTasksPagedAsync(
            int userId, int pageNumber, int pageSize, int? categoryId, string? searchTerm)
        {
            var (tasks, totalCount) = await taskRepository.GetTasksByUserIdPagedAsync(userId, pageNumber, pageSize, categoryId, searchTerm);

            var dtos = tasks.Select(MapToResponseDto);

            return new PagedResponseDto<TaskResponseDto>
            {
                Items = dtos,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalItems = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };
        }

        public async Task<TaskResponseDto?> UpdateTaskAsync(int id, UpdateTaskDto dto, int userId)
        {
            var task = await taskRepository.GetByIdAsync(id, userId);
            if (task is null) return null;

            task.Title = dto.Title.Trim();
            task.Description = dto.Description;
            task.IsCompleted = dto.IsCompleted;
            task.DueDate = dto.DueDate;
            task.CategoryId = dto.CategoryId;

            var updated = await taskRepository.UpdateAsync(task);

            return updated is null ? null : MapToResponseDto(updated);
        }

        public async Task<bool> DeleteTaskAsync(int id, int userId)
        {
            var task = await taskRepository.GetByIdAsync(id, userId);
            if (task is null) return false;

            return await taskRepository.DeleteAsync(task);
        }

        private static TaskResponseDto MapToResponseDto(TodoTask task)
        {
            return new TaskResponseDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                IsCompleted = task.IsCompleted,
                CreatedAt = task.CreatedAt,
                DueDate = task.DueDate,
                CategoryId = task.CategoryId,
                CategoryName = task.Category?.Name
            };
        }
    }
}