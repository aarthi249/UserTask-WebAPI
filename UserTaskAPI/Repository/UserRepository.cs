using Microsoft.EntityFrameworkCore;
using UserTaskAPI.Models;

namespace UserTaskAPI.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly UserTaskDbContext _context;

        public UserRepository(UserTaskDbContext context)
        {
            _context = context;
        }

        public async Task AddUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _context.Users
                         .Include(u => u.Tasks)
                         .ToListAsync();
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            return await _context.Users
                .Include(u => u.Tasks)
                .FirstOrDefaultAsync(u => u.UserId == userId);
        }
    }
}
