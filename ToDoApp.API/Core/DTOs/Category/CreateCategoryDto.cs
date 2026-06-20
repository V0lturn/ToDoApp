using System.ComponentModel.DataAnnotations;

namespace ToDoApp.Core.DTOs.Category
{
    public class CreateCategoryDto
    {
        [Required]
        [MinLength(2)]
        public string Name { get; set; } = string.Empty;
    }
}
