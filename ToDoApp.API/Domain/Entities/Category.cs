using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ToDoApp.Domain.Entities
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User? User { get; set; }

        public ICollection<TodoTask> Tasks { get; set; } = new List<TodoTask>();
    }
}
