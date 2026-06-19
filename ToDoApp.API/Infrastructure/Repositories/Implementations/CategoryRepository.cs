using Microsoft.EntityFrameworkCore;
using ToDoApp.API.Infrastructure.Repositories.Interfaces;
using ToDoApp.Domain.Entities;
using ToDoApp.Infrastructure.Data;

namespace ToDoApp.API.Infrastructure.Repositories.Implementations
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _context;

        public CategoryRepository (AppDbContext context)
        {
            _context = context;
        }

        public async Task<Category> CreateAsync(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<IEnumerable<Category>> GetByUserIdAsync(int userId)
        {
            return await _context.Categories.Where(c => c.UserId == userId).ToListAsync();
        }
    }
}
