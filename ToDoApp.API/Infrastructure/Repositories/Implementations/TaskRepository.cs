using ToDoApp.Domain.Entities;
using ToDoApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Infrastructure.Repositories.Interfaces;

namespace ToDoApp.Infrastructure.Repositories.Implementations
{
    public class TaskRepository: ITaskRepository
    {
        private readonly AppDbContext _context;

        public TaskRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<TodoTask?> GetByIdAsync(int id, int userId)
        {
            return await _context.Tasks
                .Include(t => t.Category)
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
        }

        public async Task<TodoTask> CreateAsync(TodoTask task)
        {
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();
            return task;
        }

        public async Task<IEnumerable<TodoTask>> GetTasksByUserIdAsync(int userId)
        {
            return await _context.Tasks
                .Include(t => t.Category)
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<TodoTask?> UpdateAsync(TodoTask task)
        {
            _context.Tasks.Update(task);
            await _context.SaveChangesAsync();
            return task;
        }
    }
}
