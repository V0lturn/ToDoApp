using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ToDoApp.Core.DTOs.Category;
using ToDoApp.Core.Interfaces;

namespace ToDoApp.API.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController(ICategoryService categoryService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userId = GetCurrentUserId();
            if (userId is null) return Unauthorized(new { message = "User ID not found in token" });

            var categories = await categoryService.GetUserCategoriesAsync(userId.Value);
            return Ok(categories);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCategoryDto dto)
        {
            var userId = GetCurrentUserId();
            if (userId is null) return Unauthorized(new { message = "User ID not found in token" });

            try
            {
                var newCategory = await categoryService.CreateCategoryAsync(dto, userId.Value);
                return CreatedAtAction(nameof(GetAll), newCategory);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        private int? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("id");

            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }

            return null;
        }
    }
}
