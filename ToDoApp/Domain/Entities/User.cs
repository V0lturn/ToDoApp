using System.ComponentModel.DataAnnotations;

namespace ToDoApp.Domain.Entities
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<TodoTask> Tasks { get; set; } = new List<TodoTask>();
        public ICollection<Category> Categories { get; set; } = new List<Category>();
    }
}
