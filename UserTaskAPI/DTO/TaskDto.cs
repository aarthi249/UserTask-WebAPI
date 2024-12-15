using UserTaskAPI.Enums;

namespace UserTaskAPI.DTO
{
    public class TaskDto
    {
        public int TaskId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public Status Status { get; set; }
    }
}
