using ApecMoviePortal.Services.TicketServices;
using Microsoft.AspNetCore.Mvc;

namespace ApecMoviePortal.Controllers
{
    public class TicketController : Controller
    {
        private readonly ITicketService _ticketService;

        public TicketController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        [HttpGet]
        public IActionResult Confirm(string ticketId, string token)
        {
            ViewBag.TicketId = ticketId;
            ViewBag.Token = token;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmTicket(string ticketId, string token)
        {
            if (Guid.TryParse(ticketId, out var parsedTicketId))
            {
                var result = await _ticketService.ConfirmTicketAsync(parsedTicketId, token);
                if (result)
                {
                    ViewBag.Message = "Xác nhận vé thành công!";
                }
                else
                {
                    ViewBag.Message = "Xác nhận vé thất bại!";
                }
            }
            else
            {
                ViewBag.Message = "Mã vé không hợp lệ!";
            }

            return View("Confirm");
        }
    }
}
