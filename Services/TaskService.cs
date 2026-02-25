using TaskFlowApi.Services.Contract;
using TaskFlowApi.Data;
using TaskFlowApi.Models;
using TaskFlowApi.Dtos;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskFlowApi.Services
{
    public class TaskService : ITaskService
    {
        private readonly AppDbContext _context;

        public TaskService(AppDbContext context)
        {
            _context = context;
        }

        public async Task CreateTaskAsync(TaskDetailDto request)
        {
            var task = request.ToEntity();
            _context.TaskItems.Add(task);
            await _context.SaveChangesAsync();
        }

        public async Task<List<TaskSummaryDto>> GetTasksAsync(TaskItemStatus? status = null, string? title = null, string? sortBy = null, string? sortOrder = null)
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

            return await query.Select(t => new TaskSummaryDto
            {
                Title = t.Title,
                Description = t.Description,
                Status = t.Status
            }).ToListAsync();
        }

        public async Task<TaskDetailDto?> GetTaskByIdAsync(int id)
        {
            var task = await _context.TaskItems.FindAsync(id);
            if (task == null) return null;

            return new TaskDetailDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Status = task.Status,
                CreatedAt = task.CreatedAt,
                DueDate = task.DueDate
            };
        }

        public async Task<bool> DeleteTaskAsync(int id)
        {
            var task = await _context.TaskItems.FindAsync(id);
            if (task == null) return false;

            _context.TaskItems.Remove(task);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateTaskAsync(int id, TaskDetailDto request)
        {
            var task = await _context.TaskItems.FindAsync(id);
            if (task == null) return false;

            task.Title = request.Title;
            task.Description = request.Description;
            task.Status = request.Status;
            task.DueDate = request.DueDate;

            _context.Entry(task).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}