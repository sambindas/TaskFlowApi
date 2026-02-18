using System;
using TaskFlowApi.Models;

namespace TaskFlowApi.Dtos
{
    public class TaskSummaryDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public TaskItemStatus Status { get; set; }
    }
}