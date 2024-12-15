using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Exchange.WebServices.Data;
using Swashbuckle.AspNetCore.Annotations;
using UserTaskAPI.DTO;
using UserTaskAPI.Models;
using UserTaskAPI.Repository;

namespace UserTaskAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserTaskController : ControllerBase
    {
        private readonly IUserTaskRepository _userTaskRepository;
        private readonly IMapper _mapper;

        public UserTaskController(IUserTaskRepository userTaskRepository, IMapper mapper)
        {
            _userTaskRepository = userTaskRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Creates a new task for a user.
        /// </summary>
        /// <param name="userTaskDto">Task data transfer object containing details for the new task.</param>
        /// <returns>Returns success or error message.</returns>
        /// <response code="200">Task created successfully.</response>
        /// <response code="400">Invalid user or invalid status value.</response>
        [HttpPost]
        [SwaggerOperation(Summary = "Create a new task", Description = "This endpoint allows you to create a task for a user.")]
        [SwaggerResponse(200, "Task created successfully.")]
        [SwaggerResponse(400, "Bad request due to invalid user or status.")]
        public async Task<IActionResult> CreateTask([FromQuery] UserTaskDto userTaskDto)
        {
            if (!await _userTaskRepository.UserExistsAsync(userTaskDto.UserId))
            {
                return BadRequest(new { Message = "Invalid UserId." });
            }

            if (string.IsNullOrEmpty(userTaskDto.DueDate) || !string.IsNullOrEmpty(userTaskDto.DueDate))
            {
                if (!DateTime.TryParseExact(userTaskDto.DueDate,
                                            "dd-MM-yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None,
                                            out DateTime dueDate))
                {
                    return BadRequest(new { Message = "Invalid date format. Use 'dd-MM-yyyy HH:mm:ss'." });
                }
            }

            UserTask userTask = _mapper.Map<UserTask>(userTaskDto);

            await _userTaskRepository.AddTaskAsync(userTask);

            return Ok(new { Message = "Task created successfully." });
        }

        /// <summary>
        /// Retrieves all tasks for a specific user.
        /// </summary>
        /// <param name="userId">The user ID to fetch tasks for.</param>
        /// <returns>Returns a list of tasks for the specified user.</returns>
        /// <response code="200">Tasks retrieved successfully.</response>
        /// <response code="404">No tasks found for the user.</response>
        [HttpGet("{userId}")]
        [SwaggerOperation(Summary = "Get tasks by user", Description = "This endpoint allows you to retrieve all tasks for a specified user.")]
        [SwaggerResponse(200, "Tasks retrieved successfully.", typeof(IEnumerable<UserTaskDto>))]
        [SwaggerResponse(404, "No tasks found for the user.")]
        public async Task<IActionResult> GetTasksByUser(int userId)
        {
            IEnumerable<UserTask> tasks = await _userTaskRepository.GetTasksByUserAsync(userId);
            if (!tasks.Any())
            {
                return NotFound(new { Message = "No tasks found for the user." });
            }

            IEnumerable<UserTaskDto> taskDtos = _mapper.Map<IEnumerable<UserTaskDto>>(tasks);

            return Ok(taskDtos);
        }

        /// <summary>
        /// Updates an existing task for a user.
        /// </summary>
        /// <param name="taskId">The ID of the task to be updated.</param>
        /// <param name="updatedTaskDto">The updated task data.</param>
        /// <returns>Returns success or error message.</returns>
        /// <response code="200">Task updated successfully.</response>
        /// <response code="404">Task not found.</response>
        /// <response code="400">Invalid status value.</response>
        [HttpPut("{taskId}")]
        [SwaggerOperation(Summary = "Update an existing task", Description = "This endpoint allows you to update an existing task.")]
        [SwaggerResponse(200, "Task updated successfully.")]
        [SwaggerResponse(404, "Task not found.")]
        [SwaggerResponse(400, "Invalid status value.")]
        public async Task<IActionResult> UpdateTask(int taskId, [FromQuery] UpdateUserTaskDto updatedTaskDto)
        {
            UserTask task = await _userTaskRepository.GetTaskByIdAsync(taskId);
            if (task == null)
            {
                return NotFound(new { Message = "Task not found." });
            }

            if (!string.IsNullOrEmpty(updatedTaskDto.DueDate))
            {
                if (!DateTime.TryParseExact(updatedTaskDto.DueDate,
                                            "dd-MM-yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None,
                                            out DateTime dueDate))
                {
                    return BadRequest("Invalid date format. Use 'dd-MM-yyyy HH:mm:ss'.");
                }
                task.DueDate = dueDate;
            }

            task.Title = updatedTaskDto.Title ?? task.Title;
            task.Status = updatedTaskDto.Status ?? task.Status;

            await _userTaskRepository.UpdateTaskAsync(task);

            return Ok(new { Message = "Task updated successfully." });
        }

        /// <summary>
        /// Deletes a task by ID.
        /// </summary>
        /// <param name="taskId">The ID of the task to delete.</param>
        /// <returns>Returns success or error message.</returns>
        /// <response code="200">Task deleted successfully.</response>
        /// <response code="404">Task not found.</response>
        [HttpDelete("{taskId}")]
        [SwaggerOperation(Summary = "Delete a task", Description = "This endpoint allows you to delete a task by its ID.")]
        [SwaggerResponse(200, "Task deleted successfully.")]
        [SwaggerResponse(404, "Task not found.")]
        public async Task<IActionResult> DeleteTask(int taskId)
        {
            UserTask task = await _userTaskRepository.GetTaskByIdAsync(taskId);
            if (task == null)
            {
                return NotFound(new { Message = "Task not found." });
            }

            await _userTaskRepository.DeleteTaskAsync(task);

            return Ok(new { Message = "Task deleted successfully." });
        }
    }
}
