using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskFlowApi.Data;
using TaskFlowApi.Models;
using TaskFlowApi.Dtos;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace TaskFlowApi.Controllers
{

    public class ValidateTaskExistsAttribute : ActionFilterAttribute
    {
        private readonly AppDbContext _context;

        public ValidateTaskExistsAttribute(AppDbContext context)
        {
            _context = context;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.ActionArguments.ContainsKey("id") && context.ActionArguments["id"] is int id)
            {
                var taskItem = await _context.TaskItems.FindAsync(id);
                if (taskItem == null)
                {
                    context.Result = new BadRequestObjectResult("Please provide a valid task ID.");
                    return;
                }
            }

            await next();
        }
    }

    
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TasksController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Tasks
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskItem>>> GetTaskItems(
            [FromQuery] TaskItemStatus? status, 
            [FromQuery] string? title,
            [FromQuery] string? sortBy, string? sortOrder
            )
        {
            var query = _context.TaskItems.AsQueryable();

            if (status.HasValue)
                query = query.Where(t => t.Status == status.Value);

            if (!string.IsNullOrEmpty(title))
                query = query.Where(t => t.Title.Contains(title));

            if (!string.IsNullOrEmpty(sortBy))
            {
                bool ascending = sortOrder?.ToLower() == "asc";
                query = sortBy.ToLower() switch
                {
                    "title" => ascending ? query.OrderBy(t => t.Title) : query.OrderByDescending(t => t.Title),
                    "description" => ascending ? query.OrderBy(t => t.Description) : query.OrderByDescending(t => t.Description),
                    "status" => ascending ? query.OrderBy(t => t.Status) : query.OrderByDescending(t => t.Status),
                    "duedate" => ascending ? query.OrderBy(t => t.DueDate) : query.OrderByDescending(t => t.DueDate),
                    _ => query
                };
            }

            var tasks = await query.Select(t => new TaskSummaryDto
            {
                Title = t.Title
            }).ToListAsync();

            return Ok(tasks);
        }

        // GET: api/Tasks/5
        [TypeFilter(typeof(ValidateTaskExistsAttribute))]
        [HttpGet("{id}")]
        public async Task<ActionResult<TaskItem>> GetTaskItem(int id)
        {
            var taskItem = await _context.TaskItems.FindAsync(id);

            if (taskItem == null)
            {
                return NotFound();
            }

            return taskItem;
        }

        // POST: api/Tasks
        [HttpPost]
        public async Task<ActionResult<TaskItem>> PostTaskItem(TaskItem taskItem)
        {
            _context.TaskItems.Add(taskItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTaskItem), new { id = taskItem.Id }, taskItem);
        }

        // PUT: api/Tasks/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTaskItem(int id, TaskItem taskItem)
        {
            if (id != taskItem.Id)
            Console.WriteLine("ID mismatch: " + id + " != " + taskItem.Id);
            {
                return BadRequest();
            }

            _context.Entry(taskItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TaskItemExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Tasks/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTaskItem(int id)
        {
            var taskItem = await _context.TaskItems.FindAsync(id);
            if (taskItem == null)
            {
                return NotFound();
            }

            _context.TaskItems.Remove(taskItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TaskItemExists(int id)
        {
            return _context.TaskItems.Any(e => e.Id == id);
        }
    }
}