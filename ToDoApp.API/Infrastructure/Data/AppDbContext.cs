using Microsoft.EntityFrameworkCore;
using ToDoApp.Domain.Entities;

namespace ToDoApp.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<TodoTask> Tasks { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Unique index for login
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            // When deleting a user, we delete their tasks
            modelBuilder.Entity<TodoTask>()
                .HasOne(t => t.User)
                .WithMany(u => u.Tasks)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // When deleting a category, the tasks remain, CategoryId = NULL
            modelBuilder.Entity<TodoTask>()
                .HasOne(t => t.Category)
                .WithMany(c => c.Tasks)
                .HasForeignKey(t => t.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);

            // Change cascading delete to NoAction to break a conflict cycle in SQL Server
            modelBuilder.Entity<Category>()
                .HasOne(c => c.User)
                .WithMany(u => u.Categories)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
