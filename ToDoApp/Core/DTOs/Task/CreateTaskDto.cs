namespace ToDoApp.Core.DTOs.Task
{
    public class CreateTaskDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime? DueDate { get; set; }
        public int? CategoryId { get; set; }
    }
}
