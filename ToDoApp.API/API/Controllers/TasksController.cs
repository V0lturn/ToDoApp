using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ToDoApp.Core.DTOs.Task;
using ToDoApp.Core.Interfaces;

namespace ToDoApp.API.Controllers
{
    [Authorize]                         
    [ApiController]
    [Route("api/[controller]")]
    public class TasksController(ITaskService taskService) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTaskDto dto)
        {
            var userId = GetCurrentUserId();
            if (userId is null) return Unauthorized(new { message = "User ID not found in token" });

            var result = await taskService.CreateTaskAsync(dto, userId.Value);

            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpGet]
        public async Task<IActionResult> GetTasks(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 4,
            [FromQuery] int? categoryId = null,
            [FromQuery] string? searchTerm = null)
        {
            var userId = GetCurrentUserId();
            if (userId is null) return Unauthorized(new { message = "User ID not found in token" });

            var result = await taskService.GetUserTasksPagedAsync(userId.Value, pageNumber, pageSize, categoryId, searchTerm);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var userId = GetCurrentUserId();
            if (userId is null) return Unauthorized(new { message = "User ID not found in token" });

            var task = await taskService.GetByIdAsync(id, userId.Value);
            if (task is null)
            {
                return NotFound(new { message = $"Task with ID {id} not found." });
            }

            return Ok(task);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateTaskDto dto)
        {
            var userId = GetCurrentUserId();
            if (userId is null) return Unauthorized(new { message = "User ID not found in token" });

            var result = await taskService.UpdateTaskAsync(id, dto, userId.Value);
            if (result is null)
            {
                return NotFound(new { message = $"Task with ID {id} not found or access denied." });
            }

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetCurrentUserId();
            if (userId is null) return Unauthorized(new { message = "User ID not found in token" });

            var isDeleted = await taskService.DeleteTaskAsync(id, userId.Value);
            if (!isDeleted)
            {
                return NotFound(new { message = $"Task with ID {id} not found or access denied." });
            }

            return NoContent();
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
