﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TicketServices.Application.BussinessServices;
using TicketServices.Application.ModelsDTO;
using TicketServices.Domain.Models;

namespace TicketServices.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly ITicketService _ticketService;

        public TicketController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Ticket>>> GetAllTicketsAsync()
        {
            var tickets = await _ticketService.GetAllTicketsAsync();
            return Ok(tickets);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Ticket>> GetTicketByIdAsync(Guid id)
        {
            var ticket = await _ticketService.GetTicketByIdAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }
            return Ok(ticket);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Ticket>> AddTicketAsync([FromBody] TicketDTO ticketDTO)
        {
            try
            {
                var userId = GetUserIdFromClaim();
                var ticket = await _ticketService.AddTicketAsync(ticketDTO, userId);
                return ticket;
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message); 
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error"); 
            }
        }



        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateTicketAsync(Guid id, [FromBody] TicketDTO ticketDTO)
        {
            try
            {
                await _ticketService.UpdateTicketAsync(id, ticketDTO);
                return NoContent();
            }
            catch (ArgumentException)
            {
                return NotFound();
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTicketAsync(Guid id)
        {
            await _ticketService.DeleteTicketAsync(id);
            return NoContent();
        }

        [HttpPost("confirm")]
        public async Task<IActionResult> ConfirmTicket([FromBody] TicketConfirmationDTO confirmation)
        {
            var confirmationResult = await _ticketService.ConfirmTicketAsync(confirmation.TicketId, confirmation.Token);
            if (confirmationResult)
            {
                return Ok("Ticket confirmed successfully");
            }
            else
            {
                return BadRequest("Invalid token or ticket ID");
            }
        }

        protected Guid GetUserIdFromClaim()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid Id))
                throw new UnauthorizedAccessException();
            return Id;
        }

        protected string GetUserEmailFromClaim()
        {
            var userEmailClaim = User.FindFirst(ClaimTypes.Email);
            if (userEmailClaim == null || string.IsNullOrEmpty(userEmailClaim.Value))
                throw new UnauthorizedAccessException("Email claim not found in token.");
            return userEmailClaim.Value;
        }

    }
}
