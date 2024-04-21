using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Event;
using UserServices.Application.BussinessServices;
using UserServices.Application.ModelsDTO;

namespace UserServices.Api.Controllers
{
    [Route("v1/api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMessageProducer _producer;
        public UserController(IUserService userService , IMessageProducer messageProducer)
        {
            _userService = userService;
            _producer = messageProducer;
        }

        /// <summary>
        /// get detail user by id - admin
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserDTO>> GetById(Guid id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return user;
        }

        /// <summary>
        /// get all user - admin
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetAll()
        {
            var users = await _userService.GetAllAsync();
            return Ok(users);
        }

        /// <summary>
        /// register - no auth
        /// </summary>
        [HttpPost("register")]
        public async Task<ActionResult<UserDTO>> Add(UserDTO userDTO)
        {
            var newUserDTO = await _userService.AddAsync(userDTO);
            return Ok(newUserDTO);
        }

        /// <summary>
        /// delete user by id - admin
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            await _userService.DeleteAsync(id);

            return NoContent();
        }

        /// <summary>
        /// login - no auth
        /// </summary>
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login(LoginRequest loginRequest)
        {
            try
            {
                var loginResponse = await _userService.LoginAsync(loginRequest);
                return Ok(loginResponse);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // API này sẽ dùng RabbitMQ để thao tác với API gửi mail bên mail services
        /// <summary>
        /// send mail to user - admin
        /// </summary>
        [HttpPost("send-mail")]
        public IActionResult PushToMailQueue(string mail, string subject, string bodyString)
        {
            var message = $"To: {mail}, Subject: {subject}, Body: {bodyString}";
            _producer.SendMessage(message, "sendmail");
            return Ok();
        }

        [HttpPost("refresh-tokens")]
        public async Task<IActionResult> RefreshTokens([FromBody] RefreshTokenRequest request)
        {
            try
            {
                var response = await _userService.RefreshTokensAsync(request.RefreshToken);
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi trong quá trình xử lý yêu cầu." });
            }
        }

    }
}
