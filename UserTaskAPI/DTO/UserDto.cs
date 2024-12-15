using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace UserTaskAPI.DTO
{
    public class UserDto
    {
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, MinimumLength =2, ErrorMessage = "Name must be between 2 and 100 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long.")]
        [SwaggerSchema(Description = "Password must be at least 6 characters long.")]
        public string Password { get; set; }
    }
}
