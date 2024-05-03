﻿using ApecMovieCore.BaseResponse;
using Grpc.Core;
using Grpc.Net.Client;
using GrpcEmailService;
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
        private readonly EmailSender.EmailSenderClient _emailClient;
        public UserController(IUserService userService , IMessageProducer messageProducer, EmailSender.EmailSenderClient emailClient)
        {
            _userService = userService;
            _producer = messageProducer;
            _emailClient = emailClient;
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
        public async Task<IActionResult> SendEmailAsync(string to, string subject, string body)
        {
            try
            {
                var request = new EmailRequest { To = to, Subject = subject, Body = body };
                var response = await _emailClient.SendEmailAsync(request);
                return Ok(response.Message);
            }
            catch (RpcException ex)
            {
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

        protected Guid GetUserIdFromClaim()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid Id))
                throw new UnauthorizedAccessException();
            return Id;
        }

    }
}
