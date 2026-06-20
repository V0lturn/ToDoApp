using ToDoApp.Core.DTOs.Pagination;
using ToDoApp.Core.DTOs.Task;
using ToDoApp.Core.Interfaces;
using ToDoApp.Domain.Entities;
using ToDoApp.Infrastructure.Repositories.Interfaces;

namespace ToDoApp.Core.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;

        public TaskService(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        public async Task<TaskResponseDto> CreateTaskAsync(CreateTaskDto dto, int userId)
        {
            var task = new TodoTask
            {
                Title = dto.Title,
                Description = dto.Description ?? string.Empty,
                DueDate = dto.DueDate,
                CategoryId = dto.CategoryId,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _taskRepository.CreateAsync(task);

            return new TaskResponseDto
            {
                Id = created.Id,
                Title = created.Title,
                Description = created.Description,
                IsCompleted = created.IsCompleted,
                CreatedAt = created.CreatedAt,
                DueDate = created.DueDate,
                CategoryId = created.CategoryId,
                CategoryName = created.Category?.Name
            };
        }

        public async Task<TaskResponseDto?> GetByIdAsync(int id, int userId)
        {
            var task = await _taskRepository.GetByIdAsync(id, userId);

            if (task is null)
                return null;

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

        public async Task<PagedResponseDto<TaskResponseDto>> GetUserTasksPagedAsync(
            int userId, int pageNumber, int pageSize, int? categoryId, string? searchTerm)
        {
            var (tasks, totalCount) = await _taskRepository.GetTasksByUserIdPagedAsync(userId, pageNumber, pageSize, categoryId, searchTerm);
            var dtos = tasks.Select(t => new TaskResponseDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                IsCompleted = t.IsCompleted,
                CreatedAt = t.CreatedAt,
                DueDate = t.DueDate,
                CategoryId = t.CategoryId,
                CategoryName = t.Category?.Name
            });

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
            var task = await _taskRepository.GetByIdAsync(id, userId);

            if (task is null)
                return null;

            task.Title = dto.Title;
            task.Description = dto.Description;
            task.IsCompleted = dto.IsCompleted;
            task.DueDate = dto.DueDate;
            task.CategoryId = dto.CategoryId;

            var updated = await _taskRepository.UpdateAsync(task);

            if (updated is null)
                return null;

            return new TaskResponseDto
            {
                Id = updated.Id,
                Title = updated.Title,
                Description = updated.Description,
                IsCompleted = updated.IsCompleted,
                CreatedAt = updated.CreatedAt,
                DueDate = updated.DueDate,
                CategoryId = updated.CategoryId,
                CategoryName = updated.Category?.Name
            };
        }

        public async Task<bool> DeleteTaskAsync(int id, int userId)
        {
            var task = await _taskRepository.GetByIdAsync(id, userId);

            if (task is null)
                return false;

            return await _taskRepository.DeleteAsync(task);
        }
    }
}