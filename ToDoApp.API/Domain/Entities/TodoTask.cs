using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ToDoApp.Domain.Entities
{
    public class TodoTask
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string Description { get; set; } = string.Empty;

        public bool IsCompleted { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? DueDate { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User? User { get; set; }

        public int? CategoryId { get; set; }

        [ForeignKey(nameof(CategoryId))]
        public Category? Category { get; set; }
    }
}
