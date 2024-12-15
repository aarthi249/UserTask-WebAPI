using Microsoft.EntityFrameworkCore;
using UserTaskAPI.Enums;

namespace UserTaskAPI.Models
{
    public class UserTaskDbContext : DbContext
    {
        public UserTaskDbContext(DbContextOptions<UserTaskDbContext> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<UserTask> UserTasks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserTask>()
                .HasOne(ut => ut.User)
                .WithMany(u => u.Tasks)
                .HasForeignKey(ut => ut.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserTask>()
                .Property(ut => ut.Status)
                .HasConversion(
                v => v.ToString(),
                v => (Status)Enum.Parse(typeof(Status), v));

            base.OnModelCreating(modelBuilder);
        }
    }
}
