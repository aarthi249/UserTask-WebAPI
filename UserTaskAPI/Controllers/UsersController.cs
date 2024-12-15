using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Annotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserTaskAPI.DTO;
using UserTaskAPI.Models;
using UserTaskAPI.Repository;

namespace UserTaskAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public UsersController(IUserRepository userRepository, IConfiguration configuration, IMapper mapper)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _mapper = mapper;
        }

        /// <summary>
        /// Registers a new user with the provided details.
        /// </summary>
        /// <param name="userDto">User data transfer object containing user details for registration.</param>
        /// <returns>Returns success or error message.</returns>
        /// <response code="200">User registered successfully</response>
        /// <response code="400">Bad request due to validation errors or email already in use</response>
        [HttpPost("register")]
        [SwaggerOperation(Summary = "Registers a new user", Description = "This endpoint allows you to register a new user with details.")]
        [SwaggerResponse(200, "User registered successfully.")]
        [SwaggerResponse(400, "Bad request due to validation errors.")]
        public async Task<IActionResult> Register([FromQuery] UserDto userDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                                       .SelectMany(v => v.Errors)
                                       .Select(e => e.ErrorMessage)
                                       .ToList();

                return BadRequest(new { Message = string.Join(", ", errors) });
            }

            User existingUser = await _userRepository.GetUserByEmailAsync(userDto.Email);
            if (existingUser != null)
            {
                return BadRequest(new { Message = "User already registered with this email." });
            }

            User user = _mapper.Map<User>(userDto);
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password);
            await _userRepository.AddUserAsync(user);

            return Ok(new { Message = "User registered successfully." });
        }

        /// <summary>
        /// Logs in a user and generates a JWT token.
        /// </summary>
        /// <param name="email">The email address of the user.</param>
        /// <param name="password">The password of the user.</param>
        /// <returns>Returns a JWT token if credentials are valid.</returns>
        /// <response code="200">JWT token generated</response>
        /// <response code="401">Unauthorized if invalid credentials</response>
        [HttpPost("login")]
        [SwaggerOperation(Summary = "Logs in a user and generates a JWT token", Description = "This endpoint allows a user to log in and get a JWT token.")]
        [SwaggerResponse(200, "JWT token generated.")]
        [SwaggerResponse(401, "Invalid credentials.")]
        public async Task<IActionResult> Login([FromQuery] string email, [FromQuery] string password)
        {
            User user = await _userRepository.GetUserByEmailAsync(email);
            if (user == null)
            {
                return Unauthorized(new { Message = "Invalid credentials." });
            }

            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                return Unauthorized(new { Message = "Invalid credentials." });
            }

            string token = GenerateJwtToken(user);

            return Ok(new { Token = token });
        }

        /// <summary>
        /// Retrieves all users with their tasks.
        /// </summary>
        /// <returns>Returns a list of all users along with their tasks.</returns>
        /// <response code="200">List of users and their tasks</response>
        [Authorize]
        [HttpGet]
        [SwaggerOperation(Summary = "Retrieves all users with their tasks", Description = "This endpoint retrieves a list of all users along with their tasks.")]
        [SwaggerResponse(200, "List of users and tasks.", typeof(IEnumerable<GetUsersDto>))]
        [SwaggerResponse(500, "Internal server error.")]
        public async Task<IActionResult> GetUsers()
        {
            IEnumerable<User> users = await _userRepository.GetAllUsersAsync();
            IEnumerable<GetUsersDto> userDtos = _mapper.Map<IEnumerable<GetUsersDto>>(users);

            TimeZoneInfo istTimeZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

            var response = userDtos.Select(userDto => new
            {
                userDto.Name,
                userDto.Email,
                CreatedDate = TimeZoneInfo.ConvertTimeFromUtc(userDto.CreatedDate, istTimeZone).ToString("yyyy-MM-dd HH:mm:ss"),
                Tasks = userDto.Tasks.Select(task =>
                {
                    string formattedDueDate;
                    bool isValidDueDate = DateTime.TryParseExact(
                        TimeZoneInfo.ConvertTimeFromUtc(task.DueDate, istTimeZone).ToString("dd-MM-yyyy HH:mm:ss"),
                        "dd-MM-yyyy HH:mm:ss",
                        System.Globalization.CultureInfo.InvariantCulture,
                        System.Globalization.DateTimeStyles.None,
                        out DateTime dueDate);

                    if (!isValidDueDate)
                    {
                        formattedDueDate = "Invalid date format";
                    }
                    else
                    {
                        formattedDueDate = dueDate.ToString("dd-MM-yyyy HH:mm:ss");
                    }

                    return new
                    {
                        task.TaskId,
                        task.Title,
                        task.Description,
                        DueDate = formattedDueDate,
                        task.Status
                    };
                })
            });

            return Ok(response);
        }

        /// <summary>
        /// Retrieves a specific user by their ID, including their tasks.
        /// </summary>
        /// <param name="userId">The ID of the user to retrieve.</param>
        /// <returns>Returns the user and their tasks.</returns>
        /// <response code="200">User details and tasks</response>
        /// <response code="404">User not found</response>
        [Authorize]
        [HttpGet("{userId}")]
        [SwaggerOperation(Summary = "Retrieves a specific user by ID with tasks", Description = "This endpoint retrieves a user by their ID along with tasks.")]
        [SwaggerResponse(200, "User details and tasks.", typeof(GetUsersDto))]
        [SwaggerResponse(404, "User not found.")]
        public async Task<IActionResult> GetUserById(int userId)
        {
            User user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound(new { Message = "User not found." });
            }
            GetUsersDto userDto = _mapper.Map<GetUsersDto>(user);

            TimeZoneInfo istTimeZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

            var response = new
            {
                userDto.Name,
                userDto.Email,
                CreatedDate = TimeZoneInfo.ConvertTimeFromUtc(userDto.CreatedDate, istTimeZone).ToString("yyyy-MM-dd HH:mm:ss"),
                Tasks = userDto.Tasks.Select(task => new
                {
                    task.TaskId,
                    task.Title,
                    task.Description,
                    DueDate = TimeZoneInfo.ConvertTimeFromUtc(task.DueDate, istTimeZone).ToString("yyyy-MM-dd HH:mm:ss"),
                    task.Status
                })
            };

            return Ok(response);
        }

        private string GenerateJwtToken(User userTask)
        {
            byte[] key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? "SecretKey");
            Claim[] claims =
            [
                new Claim(JwtRegisteredClaimNames.Sub, userTask.UserId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, userTask.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            ];

            SigningCredentials credentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);
            JwtSecurityToken token = new(
                issuer: _configuration["Jwt:Issuer"] ?? "UserTaskAPI",
                audience: _configuration["Jwt:Issuer"] ?? "UserTaskAPI",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
