using TaskManager.API.DTOs;
using TaskManager.API.Services;
using Microsoft.AspNetCore.Mvc;
using TaskManager.API.Shared.Enums;
using TaskStatus = TaskManager.API.Shared.Enums.TaskStatus;

namespace TaskManager.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TaskController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTaskByIdAsync(int id)
        {
            var taskDto = await _taskService.GetTaskByIdAsync(id);
            if (taskDto == null)
            {
                return NotFound(new { message = $"Task with id {id} not found." });
            }

            return Ok(taskDto);
        }

        [HttpGet]
        public async Task<IActionResult> GetTasksAsync(
            [FromQuery] TaskStatus? status,
            [FromQuery] TaskPriority? priority,
            [FromQuery] string sortBy = nameof(TaskDto.DueDate),
            [FromQuery] bool sortDescending = false,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var tasks = await _taskService.GetTasksAsync(status, priority, sortBy, sortDescending, page, pageSize);
            return Ok(tasks);
        }

        [HttpPost]
        public async Task<IActionResult> AddTaskAsync([FromBody] TaskDto taskDto)
        {
            if (taskDto == null)
            {
                return BadRequest(new { message = "Task data is invalid." });
            }

            var addedTask = await _taskService.AddTaskAsync(taskDto);

            return CreatedAtAction(nameof(GetTaskByIdAsync), new { id = addedTask.Id }, addedTask);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTaskAsync(int id, [FromBody] TaskDto updatedTaskDto)
        {
            if (updatedTaskDto == null)
            {
                return BadRequest(new { message = "Updated task data is invalid." });
            }

            var updatedTask = await _taskService.UpdateTaskAsync(id, updatedTaskDto);
            if (updatedTask == null)
            {
                return NotFound(new { message = $"Task with id {id} not found." });
            }

            return Ok(updatedTask);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchTaskTitleAsync(int id, [FromBody] string title)
        {
            if (string.IsNullOrEmpty(title))
            {
                return BadRequest(new { message = "Task title is required." });
            }

            var task = await _taskService.GetTaskByIdAsync(id);
            if (task == null)
            {
                return NotFound(new { message = $"Task with id {id} not found." });
            }

            task.Title = title;

            var updatedTask = await _taskService.UpdateTaskAsync(id, task);
            return Ok(updatedTask);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTaskAsync(int id)
        {
            var result = await _taskService.DeleteTaskAsync(id);
            if (!result)
            {
                return NotFound(new { message = $"Task with id {id} not found." });
            }

            return NoContent();
        }
    }
}
