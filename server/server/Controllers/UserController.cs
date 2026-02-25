using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using server.BLL.Intarfaces;
using server.Models;
using server.Models.DTO;
using System.Data;



// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserBLL _userBLL;
        private readonly ILogger<UserController> _logger;
        private readonly ITokenBLL _tokenBLL;

        public UserController(IUserBLL userBLL, ILogger<UserController> logger, ITokenBLL tokenBLL)
        {
            _userBLL = userBLL;
            _logger = logger;
            _tokenBLL = tokenBLL;
        }
        // GET: api/<UserController>
        [Authorize(Roles = "Manager")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserModel>>> Get()
        {
            _logger.LogInformation("Fetching all users");
            var users = await _userBLL.GetUsers();
            return Ok(users);
        }

        // GET api/<UserController>/5

        [HttpGet("{id}")]
        [Authorize(Roles = "Manager")]
        public async Task<ActionResult<UserDTO>> GetByIdUser(int id)
        {
            _logger.LogInformation($"Fetching user with id: {id}");
            var userDto = await _userBLL.GetById(id);
            return Ok(userDto);
        }

        // POST api/<UserController>
        [HttpPost("register")]

        public async Task<ActionResult> Register([FromBody] UserDTO userDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _logger.LogInformation($"Registering new user: {userDTO.Email}");
            _logger.LogDebug($"Add user debug:{userDTO.FirstName} - {userDTO.LastName} ");
            var authUser = await _userBLL.Register(userDTO);
            var token = _tokenBLL.GenerateToken(authUser);
            return Ok(new { token = token, message = "User created successfully" });
        }

          

        // Login api/<UserController>
        [HttpPost("login")]
        public async Task<ActionResult<string>> Login([FromBody] LoginDTO loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            // 1. קריאה ל-Service לקבלת הטוקן
            var user = await _userBLL.Login(loginDto.Email, loginDto.Password);

            // 2. אם חזר null - הפרטים לא נכונים
            if (user == null)
            {
                _logger.LogWarning($"Login failed for email: {loginDto.Email}");
                return Unauthorized("מייל או סיסמה שגויים");
            }

            var tokenString = _tokenBLL.GenerateToken(user);

            // 3. החזרת הטוקן לקליינט בתוך אובייקט JSON
            return Ok(new
            {
                Token = tokenString,
                UserName = user.FirstName,
            });
        }

    }
}