using ApecMoviePortal.Client;
using ApecMoviePortal.Services.TicketServices;
using Microsoft.AspNetCore.Mvc;

namespace ApecMoviePortal.Controllers
{
    public class PaymentController : Controller
    {
        private readonly PaypalClient _paypalClient;
        private readonly ITicketService _ticketService;
        public PaymentController(PaypalClient paypalClient, ITicketService ticketService)
        {
            this._paypalClient = paypalClient;
            _ticketService = ticketService;
        }

        public async Task<IActionResult> Index()
        {
            var token = Request.Cookies["AccessToken"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Auth");
            }

            var ticket = await _ticketService.GetUnpaidTicketsAsync(token);
            // ViewBag.ClientId is used to get the Paypal Checkout javascript SDK
            ViewBag.ClientId = _paypalClient.ClientId;
            return View(ticket);
        }

        [HttpPost]
        public async Task<IActionResult> Order(CancellationToken cancellationToken)
        {
            try
            {
                var token = Request.Cookies["AccessToken"];
                if (string.IsNullOrEmpty(token))
                {
                    return RedirectToAction("Login", "Auth");
                }

                var tickets = await _ticketService.GetUnpaidTicketsAsync(token);
                if (tickets == null || !tickets.Any())
                {
                    return BadRequest("No unpaid tickets available.");
                }

                var ticket = tickets.First();
                var price = "100.00"; // Or calculate based on the ticket details
                var currency = "USD";
                var reference = ticket.Id.ToString();

                var response = await _paypalClient.CreateOrder(price, currency, reference);

                return Ok(response);
            }
            catch (Exception e)
            {
                var error = new
                {
                    e.GetBaseException().Message
                };

                return BadRequest(error);
            }
        }


        public async Task<IActionResult> Capture(string orderId, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _paypalClient.CaptureOrder(orderId);

                var reference = response.purchase_units[0].reference_id;

                // Assuming reference_id is the ticket ID
                Guid ticketId;
                if (Guid.TryParse(reference, out ticketId))
                {
                    var markAsPaidResult = await _ticketService.MarkTicketAsPaidAsync(ticketId);
                    if (!markAsPaidResult)
                    {
                        return BadRequest("Failed to update ticket status.");
                    }
                }
                else
                {
                    return BadRequest("Invalid ticket reference ID.");
                }

                return Ok(response);
            }
            catch (Exception e)
            {
                var error = new
                {
                    e.GetBaseException().Message
                };

                return BadRequest(error);
            }
        }


        public IActionResult Success()
        {
            return View();
        }
    }
}
