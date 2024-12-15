using Microsoft.EntityFrameworkCore;
using UserTaskAPI.Models;

namespace UserTaskAPI.Repository
{
    public class UserTaskRepository : IUserTaskRepository
    {
        private readonly UserTaskDbContext _context;

        public UserTaskRepository(UserTaskDbContext context)
        {
            _context = context;
        }

        public async Task AddTaskAsync(UserTask task)
        {
            await _context.UserTasks.AddAsync(task);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTaskAsync(UserTask task)
        {
            _context.UserTasks.Remove(task);
            await _context.SaveChangesAsync();
        }

        public async Task<UserTask> GetTaskByIdAsync(int taskId)
        {
            return _context.UserTasks.FirstOrDefault(t => t.TaskId == taskId);
        }

        public async Task<IEnumerable<UserTask>> GetTasksByUserAsync(int userId)
        {
            return await _context.UserTasks.Where(t => t.UserId == userId).ToListAsync();
        }

        public async Task UpdateTaskAsync(UserTask task)
        {
            _context.UserTasks.Update(task);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UserExistsAsync(int userId)
        {
            return await _context.Users.AnyAsync(u => u.UserId == userId);
        }
    }
}
