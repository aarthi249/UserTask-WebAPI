using Swashbuckle.AspNetCore.Annotations;
using UserTaskAPI.Enums;

namespace UserTaskAPI.DTO
{
    public class UpdateUserTaskDto
    {
        public string? Title { get; set; }

        public Status? Status { get; set; }


        [SwaggerSchema(Description = "Format: 'dd-MM-yyyy HH:mm:ss'. Example: '25-12-2024 15:30:00'")]
        public string? DueDate { get; set; }
    }
}
