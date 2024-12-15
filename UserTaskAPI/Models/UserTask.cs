using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UserTaskAPI.Enums;

namespace UserTaskAPI.Models
{
    public class UserTask
    {
        [Key]
        public int TaskId { get; set; }

        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public Status Status { get; set; }
        public User User { get; set; } 
    }
}
