using TaskManager.API.DTOs;
using TaskManager.API.Shared.Enums;
using TaskStatus = TaskManager.API.Shared.Enums.TaskStatus;

namespace TaskManager.API.Services
{
    public interface ITaskService
    {
        Task<TaskDto> GetTaskByIdAsync(int id);

        Task<IEnumerable<TaskDto>> GetTasksAsync
        (
            TaskStatus? status = null,
            TaskPriority? priority = null,
            string sortBy = nameof(TaskDto.DueDate),
            bool sortDescending = false,
            int page = 1,
            int pageSize = 10
        );

        Task<TaskDto> AddTaskAsync(TaskDto taskDto);

        Task<TaskDto> UpdateTaskAsync(int id, TaskDto updatedTaskDto);

        Task<bool> DeleteTaskAsync(int id);
    }
}
