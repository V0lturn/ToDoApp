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

        public async Task<(IEnumerable<TodoTask> Items, int TotalCount)> GetTasksByUserIdPagedAsync(
            int userId, int pageNumber, int pageSize, int? categoryId, string? searchTerm)
            {
                var query = _context.Tasks
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
            _context.Tasks.Update(task);
            await _context.SaveChangesAsync();
            return task;
        }

        public async Task<bool> DeleteAsync(TodoTask task)
        {
            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
