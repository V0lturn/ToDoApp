using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ToDoApp.API.Core.DTOs.Category;
using ToDoApp.API.Core.Interfaces;

namespace ToDoApp.API.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService) => _categoryService = categoryService;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var categories = await _categoryService.GetUserCategoriesAsync(userId);
            return Ok(categories);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCategoryDto dto)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var newCategory = await _categoryService.CreateCategoryAsync(dto, userId);
            return CreatedAtAction(nameof(GetAll), newCategory);
        }
    }
}
