using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TicketServices.Application.BussinessServices;
using TicketServices.Application.ModelsDTO;

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
        public async Task<ActionResult<IEnumerable<TicketDTO>>> GetAllTicketsAsync()
        {
            var tickets = await _ticketService.GetAllTicketsAsync();
            return Ok(tickets);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TicketDTO>> GetTicketByIdAsync(Guid id)
        {
            var ticket = await _ticketService.GetTicketByIdAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }
            return Ok(ticket);
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> AddTicketAsync([FromBody] TicketDTO ticketDTO)
        {
            var ticketId = await _ticketService.AddTicketAsync(ticketDTO);
            return ticketId;
        }


        [HttpPut("{id}")]
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
    }
}
