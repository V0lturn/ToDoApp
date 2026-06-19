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
        public async Task<ActionResult<IEnumerable<TaskResponseDto>>> GetAll()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized(new { message = "User ID not found in token" });
            }

            int userId = int.Parse(userIdClaim.Value);

            var result = await _taskService.GetUserTasksAsync(userId);

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
    }
}
