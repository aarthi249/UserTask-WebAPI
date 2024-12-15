using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using UserTaskAPI.Enums;

namespace UserTaskAPI.DTO
{
    public class UserTaskDto
    {

        [Required]
        public int UserId { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public Status Status { get; set; }

        [Required]
        [SwaggerSchema(Description = "Format: 'dd-MM-yyyy HH:mm:ss'. Example: '25-12-2024 15:30:00'")]
        public string DueDate { get; set; }  
    }
}
