using EmailServices.Api.Services;
using Hangfire;
using Microsoft.AspNetCore.Mvc;

namespace EmailServices.Api.Controllers
{
    [Route("v1/api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IEmailService _emailService;
        private readonly IBackgroundJobClient _backgroundJobClient;
        public EmailController(IBackgroundJobClient backgroundJobClient,IEmailService emailService)
        {
            _backgroundJobClient = backgroundJobClient;
            _emailService = emailService;
        }

        /// <summary>
        /// send email to user
        /// </summary>
        [HttpPost]
        public IActionResult Send(string mail, string subject ,string bodyString)
        {
            _backgroundJobClient.Enqueue(() => _emailService.SendEmail(mail,subject ,bodyString));
            return Ok();
        }
    }
}
