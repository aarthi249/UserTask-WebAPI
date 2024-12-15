using System.ComponentModel.DataAnnotations;

namespace UserTaskAPI.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public ICollection<UserTask> Tasks { get; set; }
    }
}
