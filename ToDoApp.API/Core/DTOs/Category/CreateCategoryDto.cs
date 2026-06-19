using System.ComponentModel.DataAnnotations;

namespace ToDoApp.API.Core.DTOs.Category
{
    public class CreateCategoryDto
    {
        [Required]
        [MinLength(2)]
        public string Name { get; set; } = string.Empty;
    }
}
