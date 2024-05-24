using ApecMoviePortal.Models;
using ApecMoviePortal.Services.MovieServices;
using ApecMoviePortal.Services.TicketServices;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ApecMoviePortal.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMovieService _movieService;
        private readonly ITicketService _ticketService;
        public HomeController(ILogger<HomeController> logger, IMovieService movieService, ITicketService ticketService)
        {
            _movieService = movieService;
            _ticketService = ticketService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Moive()
        {
            var movieResponse = await _movieService.GetMoviesAsync();
            return View(movieResponse.Content);
        }

        public async Task<IActionResult> DetailMovie(Guid id)
        {
            var movie = await _movieService.GetMovieByIdAsync(id);
            ViewBag.Movie = movie;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DetailMovie(TicketViewModel model)
        {
            if (ModelState.IsValid)
            {
                var token = Request.Cookies["AccessToken"];
                var result = await _ticketService.BookTicketAsync(model, token);
                if (result)
                {
                    TempData["Message"] = "Đặt vé thành công! Hãy kiểm tra email để xác nhận vé";
                    return RedirectToAction("Success", "Payment");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Đặt vé thất bại, vui lòng thử lại.");
                }
            }
            return View(model);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
