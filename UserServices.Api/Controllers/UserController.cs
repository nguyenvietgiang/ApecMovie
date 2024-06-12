using ApecMovieCore.BaseResponse;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Event;
using Syncfusion.XlsIO;
using System.Security.Claims;
using UserServices.Api.Services;
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
        private readonly ExcelTemplateService _excelTemplateService;
        public UserController(IUserService userService , IMessageProducer messageProducer, EmailSenderClient emailSenderClient, ILogger<UserController> logger, IHttpContextAccessor httpContextAccessor, ExcelTemplateService excelTemplateService)
        {
            _userService = userService;
            _producer = messageProducer;
            _emailSenderClient = emailSenderClient;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _excelTemplateService = excelTemplateService;
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

        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File không hợp lệ.");

            List<UserDTO> users;
            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                users = ReadUsersFromExcel(stream);
            }

            foreach (var user in users)
            {
                await _userService.AddAsync(user);
            }

            return Ok(users.Count + " users have been added successfully.");
        }

        [HttpGet("download/{templateName}")]
        public IActionResult DownloadTemplate(string templateName)
        {
            try
            {
                byte[] fileContents = _excelTemplateService.GetExcelTemplate(templateName);
                return File(fileContents, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", templateName + ".xlsx");
            }
            catch (FileNotFoundException)
            {
                return NotFound("Template not found.");
            }
        }

        // dùng để đọc khi thêm mới bản ghi
        private List<UserDTO> ReadUsersFromExcel(Stream stream)
        {
            List<UserDTO> users = new List<UserDTO>();
            using (ExcelEngine excelEngine = new ExcelEngine())
            {
                IApplication application = excelEngine.Excel;
                application.DefaultVersion = ExcelVersion.Excel2016;

                stream.Position = 0;
                IWorkbook workbook = application.Workbooks.Open(stream);
                IWorksheet worksheet = workbook.Worksheets[0];

                int rowCount = worksheet.Rows.Length;
                for (int row = 2; row <= rowCount; row++) // hàng đầu tiền là header
                {
                    UserDTO user = new UserDTO
                    {
                        Name = worksheet[row, 1].Text,
                        Email = worksheet[row, 2].Text,
                        Password = worksheet[row, 3].Text
                    };
                    users.Add(user);
                }
            }
            return users;
        }


        [HttpPost("update")]
        public async Task<IActionResult> Update(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File không hợp lệ.");

            List<(Guid Id, UserDTO UserDTO)> users;
            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                users = ReadUsersForUpdateFromExcel(stream);
            }

            foreach (var (id, user) in users)
            {
                await _userService.UpdateAsync(id, user);
            }

            return Ok(users.Count + " users have been updated successfully.");
        }

        // dùng để đọc khi thay đổi bản ghi
        private List<(Guid Id, UserDTO UserDTO)> ReadUsersForUpdateFromExcel(Stream stream)
        {
            List<(Guid, UserDTO)> users = new List<(Guid, UserDTO)>();
            using (ExcelEngine excelEngine = new ExcelEngine())
            {
                IApplication application = excelEngine.Excel;
                application.DefaultVersion = ExcelVersion.Excel2016;

                stream.Position = 0;
                IWorkbook workbook = application.Workbooks.Open(stream);
                IWorksheet worksheet = workbook.Worksheets[0];

                int rowCount = worksheet.Rows.Length;
                for (int row = 2; row <= rowCount; row++) // Assuming first row is header
                {
                    Guid id = Guid.Parse(worksheet[row, 1].Text);
                    UserDTO user = new UserDTO
                    {
                        Name = worksheet[row, 2].Text,
                        Email = worksheet[row, 3].Text,
                        Password = worksheet[row, 4].Text
                    };
                    users.Add((id, user));
                }
            }
            return users;
        }


    }
}
