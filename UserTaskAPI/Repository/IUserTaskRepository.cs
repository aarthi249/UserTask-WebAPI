using UserTaskAPI.Models;

namespace UserTaskAPI.Repository
{
    public interface IUserTaskRepository
    {
        Task AddTaskAsync(UserTask task);
        Task<UserTask> GetTaskByIdAsync(int taskId);
        Task<IEnumerable<UserTask>> GetTasksByUserAsync(int userId);
        Task UpdateTaskAsync(UserTask task);
        Task DeleteTaskAsync(UserTask task);
        Task<bool> UserExistsAsync(int userId);
    }
}
