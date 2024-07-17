using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Event;
using System.Reactive.Subjects;
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
        private readonly IMessageProducer _producer;

        public TicketController(ITicketService ticketService, IMessageProducer messageProducer)
        {
            _ticketService = ticketService;
            _producer = messageProducer;
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
                var email = GetUserEmailFromClaim();
                string subject = "Thanh you from APEC MOVIE";
                var ticket = await _ticketService.AddTicketAsync(ticketDTO, userId);

                var message = $"To: {email}, Subject: {subject}, TickedID: {ticket.Id}, Token:{ticket.Token}";
                _producer.SendMessage(message, "verifyticket");

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
        [Authorize]
        public async Task<IActionResult> ConfirmTicket([FromBody] TicketConfirmationDTO confirmation)
        {
            var userId = GetUserIdFromClaim();
            var confirmationResult = await _ticketService.ConfirmTicketAsync(confirmation.TicketId, confirmation.Token, userId);
            if (confirmationResult)
            {
                var date = DateTime.UtcNow;
                var message = $"NewTicket is payed {date}";
                _producer.SendMessage(message, "financeUpdate");
                return Ok("Ticket confirmed successfully");
            }
            else
            {
                return BadRequest("Invalid token or ticket ID");
            }
        }

        [HttpGet("unpaid-ticket")]
        [Authorize]
        public async Task<IActionResult> GetUnpaidTickets()
        {
            var userId = GetUserIdFromClaim();
            var tickets = await _ticketService.GetUnpaidTicketsByUserAsync(userId);
            if (tickets == null)
            {
                return NotFound();
            }
            return Ok(tickets);
        }

        [HttpPost("markAsPaid/{ticketId}")]
        public async Task<IActionResult> MarkTicketAsPaid(Guid ticketId)
        {
            var result = await _ticketService.MarkTicketAsPaidAsync(ticketId);
            if (!result)
            {
                return BadRequest("Failed to update ticket status.");
            }
            return Ok("Ticket status updated successfully.");
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
