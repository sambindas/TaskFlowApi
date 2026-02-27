using System;
using TaskFlowApi.Models;
using System.ComponentModel.DataAnnotations;

namespace TaskFlowApi.Dtos
{
    public class TaskSummaryDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description {get; set;}
        public TaskItemStatus Status { get; set; }
    }


    public class CreateTaskDto
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 200 characters")]
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        [EnumDataType(typeof(TaskItemStatus), ErrorMessage = "Invalid status value")]
        public TaskItemStatus Status { get; set; }
        [DataType(DataType.DateTime, ErrorMessage = "Invalid date format")]
        public DateTime CreatedAt { get; set; }
        // public DateTime UpdatedAt { get; set; }
        [DataType(DataType.DateTime)]
        [FutureDate(ErrorMessage = "Due date must be in the future")]
        public DateTime? DueDate { get; set; }
    }

    public static class TaskMapper 
    {
        public static TaskItem ToEntity(this CreateTaskDto dto)
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