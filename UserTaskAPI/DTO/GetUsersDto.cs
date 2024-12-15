namespace UserTaskAPI.DTO
{
    public class GetUsersDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public List<TaskDto> Tasks { get; set; }
    }
}
