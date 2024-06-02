using ApecMovieCore.BaseResponse;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Event;
using System.Security.Claims;
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
        private readonly EmailSenderClient _emailSenderClient;
        private readonly ILogger<UserController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserController(IUserService userService , IMessageProducer messageProducer, EmailSenderClient emailSenderClient, ILogger<UserController> logger, IHttpContextAccessor httpContextAccessor)
        {
            _userService = userService;
            _producer = messageProducer;
            _emailSenderClient = emailSenderClient;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Response<UserDTO>>> GetById(Guid id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound(new Response<UserDTO>(404, "User not found", null));
            }
            return Ok(new Response<UserDTO>(200, "Success", user));
        }

        [HttpGet("profile")]
        public async Task<ActionResult<Response<UserDTO>>> GetProfile()
        {
            try
            {
                var userId = GetUserIdFromClaim();
                var user = await _userService.GetByIdAsync(userId);

                if (user == null)
                {
                    return NotFound(new Response<UserDTO>(404, "User not found", null));
                }

                return Ok(new Response<UserDTO>(200, "Success", user));
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new Response<UserDTO>(401, "Unauthorized", null));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Response<UserDTO>(500, $"Internal server error: {ex.Message}", null));
            }
        }


        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Response<IEnumerable<UserDTO>>>> GetAll()
        {
            var users = await _userService.GetAllAsync();
            return Ok(new Response<IEnumerable<UserDTO>>(200, "Success", users));
        }

        [HttpPost("register")]
        public async Task<ActionResult<Response<UserDTO>>> Add(UserDTO userDTO)
        {
            var newUserDTO = await _userService.AddAsync(userDTO);
            return Ok(new Response<UserDTO>(200, "Success", newUserDTO));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound(new Response<object>(404, "User not found", null));
            }

            await _userService.DeleteAsync(id);

            return NoContent();
        }

        [HttpPost("login")]
        public async Task<ActionResult<Response<LoginResponse>>> Login(LoginRequest loginRequest)
        {
            try
            {
                var loginResponse = await _userService.LoginAsync(loginRequest);
                return Ok(loginResponse);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new Response<LoginResponse>(401, ex.Message, null));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Response<LoginResponse>(500, $"Internal server error: {ex.Message}", null));
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

        [HttpPost("send-mail-grpc")]
        public async Task<IActionResult> SendEmail(string to, string subject, string body)
        {
            try
            {
                _logger.LogInformation("Sending email: To={To}, Subject={Subject}, Body={Body}", to, subject, body);
                var response = await _emailSenderClient.SendEmailAsync(to, subject, body);
                _logger.LogInformation("Email sent successfully: {Message}", response.Message);

                return Ok(response.Message);
            }
            catch (RpcException ex)
            {
                // Ghi log khi có lỗi xảy ra
                _logger.LogError(ex, "Error occurred while sending email");
                return StatusCode((int)ex.StatusCode, ex.Status.Detail);
            }
        }

        [HttpPost("refresh-tokens")]
        public async Task<ActionResult<Response<LoginResponse>>> RefreshTokens(RefreshTokenRequest Token)
        {
            try
            {
                var response = await _userService.RefreshTokensAsync(Token.RefreshToken);
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new Response<LoginResponse>(401, ex.Message, null));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Response<LoginResponse>(500, $"Internal server error: {ex.Message}", null));
            }
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            // Lấy access token từ header
            var accessToken = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];

            if (string.IsNullOrEmpty(accessToken))
            {
                return BadRequest("Access token is missing");
            }

            // Gọi phương thức logout
            var response = await _userService.LogoutAsync(accessToken);

            if (response.StatusCode == 200)
            {
                return Ok(response);
            }
            else
            {
                return StatusCode(response.StatusCode, response);
            }
        }

        protected Guid GetUserIdFromClaim()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid Id))
                throw new UnauthorizedAccessException();
            return Id;
        }

    }
}
