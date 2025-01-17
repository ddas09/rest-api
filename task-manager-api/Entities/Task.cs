using TaskManager.API.Shared.Enums;
using TaskStatus = TaskManager.API.Shared.Enums.TaskStatus;

namespace TaskManager.API.Entities
{
    public class Task
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public TaskStatus Status { get; set; } = TaskStatus.Pending;

        public TaskPriority Priority { get; set; } = TaskPriority.Low;

        public DateTime? DueDate { get; set; }
    }
}
