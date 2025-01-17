using TaskManager.API.DTOs;
using TaskManager.API.Shared.Enums;
using Task = TaskManager.API.Entities.Task;
using SystemTask = System.Threading.Tasks.Task;
using TaskStatus = TaskManager.API.Shared.Enums.TaskStatus;

namespace TaskManager.API.Services
{
    public class TaskService : ITaskService
    {
        private static int LastTaskId = 0;

        private readonly List<Task> _tasks;

        public TaskService()
        {
            this._tasks = new List<Task>();
            GenerateMockTaskData();
        }

        public async Task<TaskDto> GetTaskByIdAsync(int id)
        {
            var task = _tasks.FirstOrDefault(t => t.Id == id) ?? throw new Exception("Task with id doesn't exists");
            return await SystemTask.FromResult(MapToDto(task));
        }

        public async Task<IEnumerable<TaskDto>> GetTasksAsync
        (
            TaskStatus? status = null,
            TaskPriority? priority = null,
            string sortBy = nameof(TaskDto.DueDate),
            bool sortDescending = false,
            int page = 1,
            int pageSize = 10
        )
        {
            var query = _tasks.AsEnumerable();

            if (status.HasValue)
            {
                query = query.Where(t => t.Status == status.Value);
            }

            if (priority.HasValue)
            {
                query = query.Where(t => t.Priority == priority.Value);
            }

            var propertyInfo = typeof(Task).GetProperty(sortBy);
            if (propertyInfo == null)
            {
                Console.WriteLine($"Invalid sort column: {sortBy}. Falling back to default sorting by 'DueDate'.");
                propertyInfo = typeof(Task).GetProperty(nameof(Task.DueDate));
            }

            query = sortDescending
                ? query.OrderByDescending(t => propertyInfo!.GetValue(t, null))
                : query.OrderBy(t => propertyInfo!.GetValue(t, null));

            // Pagination
            var pagedTasks = query.Skip((page - 1) * pageSize).Take(pageSize);

            return await SystemTask.FromResult(pagedTasks.Select(MapToDto));
        }

        public async Task<TaskDto> AddTaskAsync(TaskDto taskDto)
        {
            var newTask = new Task
            {
                Id = ++LastTaskId,
                Title = taskDto.Title,
                Description = taskDto.Description,
                Status = taskDto.Status,
                Priority = taskDto.Priority,
                DueDate = taskDto.DueDate
            };

            _tasks.Add(newTask);

            return await SystemTask.FromResult(MapToDto(newTask));
        }

        public async Task<TaskDto> UpdateTaskAsync(int id, TaskDto updatedTaskDto)
        {
            var task = _tasks.FirstOrDefault(t => t.Id == id) ?? throw new Exception("Task with id doesn't exists");

            task.Title = updatedTaskDto.Title;
            task.Description = updatedTaskDto.Description;
            task.Status = updatedTaskDto.Status;
            task.Priority = updatedTaskDto.Priority;
            task.DueDate = updatedTaskDto.DueDate;

            return await SystemTask.FromResult(MapToDto(task));
        }

        public async Task<bool> DeleteTaskAsync(int id)
        {
            var task = _tasks.FirstOrDefault(t => t.Id == id);
            if (task == null)
            {
                return false;
            }

            _tasks.Remove(task);
            return await SystemTask.FromResult(true);
        }

        // Helper methods
        private void GenerateMockTaskData()
        {
            var random = new Random();

            for (int i = 1; i <= 50; i++)
            {
                var task = new Task
                {
                    Id = ++LastTaskId,
                    Title = $"Test Task {i}",
                    Description = $"Description {i}",
                    Status = (TaskStatus)random.Next(0, 3),
                    Priority = (TaskPriority)random.Next(0, 3),
                    DueDate = DateTime.Now.AddDays(random.Next(1, 30))
                };

                _tasks.Add(task);
            }
        }

        private TaskDto MapToDto(Task task)
        {
            return new TaskDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Status = task.Status,
                Priority = task.Priority,
                DueDate = task.DueDate
            };
        }
    }
}
