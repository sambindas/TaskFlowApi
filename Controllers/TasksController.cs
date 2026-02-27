using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskFlowApi.Dtos;
using TaskFlowApi.Models;
using TaskFlowApi.Services.Contract;

namespace TaskFlowApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _service;

        public TasksController(ITaskService service)
        {
            _service = service;
        }

        // POST: api/Tasks
        [HttpPost]
        public async Task<IActionResult> AddTask([FromBody] CreateTaskDto request)
        {
            await _service.CreateTaskAsync(request);
            return Ok();
        }

        // GET: api/Tasks
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskSummaryDto>>> GetTasks(
            [FromQuery] TaskItemStatus? status,
            [FromQuery] string? title,
            [FromQuery] string? sortBy,
            [FromQuery] string? sortOrder
        )
        {
            var tasks = await _service.GetTasksAsync(status, title, sortBy, sortOrder);
            return Ok(tasks);
        }

        // GET: api/Tasks/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CreateTaskDto>> GetTask(int id)
        {
            var task = await _service.GetTaskByIdAsync(id);
            if (task == null) return NotFound();
            return Ok(task);
        }

        // PUT: api/Tasks/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, [FromBody] CreateTaskDto request)
        {
            var updated = await _service.UpdateTaskAsync(id, request);
            if (!updated) return NotFound();
            return NoContent();
        }

        // DELETE: api/Tasks/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var deleted = await _service.DeleteTaskAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}