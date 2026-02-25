namespace TaskFlowApi.Services.Contract
{
    using TaskFlowApi.Dtos;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using TaskFlowApi.Models;

    public interface ITaskService
    {
        Task CreateTaskAsync(TaskDetailDto request);
        Task<List<TaskSummaryDto>> GetTasksAsync(TaskItemStatus? status = null, string? title = null, string? sortBy = null, string? sortOrder = null);
        Task<TaskDetailDto?> GetTaskByIdAsync(int id);
        Task<bool> DeleteTaskAsync(int id);
        Task<bool> UpdateTaskAsync(int id, TaskDetailDto request);
    }
}