using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ToDoApp.API.Core.DTOs.Task;
using ToDoApp.Core.DTOs.Task;
using ToDoApp.Core.Interfaces;

namespace ToDoApp.API.API.Controllers
{
    [Authorize]                         
    [ApiController]
    [Route("api/[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TasksController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTaskDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return Unauthorized(new { message = "User ID not found in token" });
            }

            int userId = int.Parse(userIdClaim.Value);

            var result = await _taskService.CreateTaskAsync(dto, userId);

            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpGet]
        public async Task<IActionResult> GetTasks(
            [FromQuery] int pageNumber = 1, 
            [FromQuery] int pageSize = 4, 
            [FromQuery] int? categoryId = null, 
            [FromQuery] string? searchTerm = null)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("id");
            if (userIdClaim == null) return Unauthorized();

            int userId = int.Parse(userIdClaim.Value);

            var result = await _taskService.GetUserTasksPagedAsync(userId, pageNumber, pageSize, categoryId, searchTerm);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized(new { message = "User ID not found in token" });
            }

            int userId = int.Parse(userIdClaim.Value);

            var task = await _taskService.GetByIdAsync(id, userId);

            if (task is null)
            {
                return NotFound(new { message = $"Task with ID {id} not found." });
            }

            return Ok(task);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<TaskResponseDto>> Update(int id, [FromBody] UpdateTaskDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized(new { message = "User ID not found in token" });
            }

            int userId = int.Parse(userIdClaim.Value);

            var result = await _taskService.UpdateTaskAsync(id, dto, userId);

            if (result is null)
            {
                return NotFound(new { message = $"Task with ID {id} not found or access denied." });
            }

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized(new { message = "User ID not found in token" });
            }

            int userId = int.Parse(userIdClaim.Value);

            var isDeleted = await _taskService.DeleteTaskAsync(id, userId);

            if (!isDeleted)
            {
                return NotFound(new { message = $"Task with ID {id} not found or access denied." });
            }

            return NoContent();
        }
    }
}
