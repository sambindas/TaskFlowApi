using System;
using TaskFlowApi.Models;

namespace TaskFlowApi.Dtos
{
    public class TaskSummaryDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description {get; set;}
        public TaskItemStatus Status { get; set; }
    }


    public class TaskDetailDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public TaskItemStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        // public DateTime UpdatedAt { get; set; }

        public DateTime? DueDate { get; set; }
    }

    public static class TaskMapper 
    {
        public static TaskItem ToEntity(this TaskDetailDto dto)
        {
            return new TaskItem
            {
                Id = dto.Id,
                Title = dto.Title,
                Description = dto.Description,
                Status = dto.Status,
                CreatedAt = DateTime.UtcNow,
                // UpdatedAt = dto.UpdatedAt,
                DueDate = dto.DueDate
            };
        }    
    }
}