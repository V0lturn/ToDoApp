using ToDoApp.Core.DTOs;
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
                Description = dto.Description,
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

        public async Task<IEnumerable<TaskResponseDto>> GetUserTasksAsync(int userId)
        {
            var tasks = await _taskRepository.GetTasksByUserIdAsync(userId);

            return tasks.Select(t => new TaskResponseDto
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
        }
    }
}