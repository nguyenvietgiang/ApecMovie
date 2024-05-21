using ApecMoviePortal.Models;
using ApecMoviePortal.Services.MovieServices;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ApecMoviePortal.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMovieService _movieService;

        public HomeController(ILogger<HomeController> logger, IMovieService movieService)
        {
            _movieService = movieService;
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

        public IActionResult DetailMovie()
        {
            return View(); 
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
