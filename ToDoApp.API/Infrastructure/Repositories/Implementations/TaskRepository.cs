using ToDoApp.Domain.Entities;
using ToDoApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Infrastructure.Repositories.Interfaces;

namespace ToDoApp.Infrastructure.Repositories.Implementations
{
    public class TaskRepository(AppDbContext context) : ITaskRepository
    {
        public async Task<TodoTask?> GetByIdAsync(int id, int userId)
        {
            return await context.Tasks
                .Include(t => t.Category)
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
        }

        public async Task<TodoTask> CreateAsync(TodoTask task)
        {
            context.Tasks.Add(task);
            await context.SaveChangesAsync();
            return task;
        }

        public async Task<(IEnumerable<TodoTask> Items, int TotalCount)> GetTasksByUserIdPagedAsync(
            int userId, int pageNumber, int pageSize, int? categoryId, string? searchTerm)
        {
            var query = context.Tasks
                .AsNoTracking()
                .Include(t => t.Category)
                .Where(t => t.UserId == userId);

            if (categoryId.HasValue)
            {
                query = query.Where(t => t.CategoryId == categoryId.Value);
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim().ToLower();
                query = query.Where(t => t.Title.ToLower().Contains(term) ||
                                         t.Description.ToLower().Contains(term));
            }

            int totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(t => t.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<TodoTask?> UpdateAsync(TodoTask task)
        {
            context.Tasks.Update(task);
            await context.SaveChangesAsync();
            return task;
        }

        public async Task<bool> DeleteAsync(TodoTask task)
        {
            context.Tasks.Remove(task);
            await context.SaveChangesAsync();
            return true;
        }
    }
}