using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToDoApp.Core.DTOs.Task;
using ToDoApp.Core.Interfaces;
using ToDoApp.Domain.Entities;

namespace ToDoApp.API.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    // [Authorize]                         
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
            int temporaryUserId = 1;

            var result = await _taskService.CreateTaskAsync(dto, temporaryUserId);
            return CreatedAtAction(nameof(Create), new { id = result.Id }, result);
        }

        //[HttpGet("{id}")]
        //public async Task<IActionResult> GetById(int id)
        //{
        //    var result = await _taskService.GetByIdAsync(id, GetUserId);

        //    if (result is null)
        //        return NotFound(new { message = $"Task with id {id} not found" });

        //    return Ok(result);
        //}
    }
}
