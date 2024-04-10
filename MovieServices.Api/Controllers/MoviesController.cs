using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MovieServices.Application.BussinessServices;
using MovieServices.Application.ModelsDTO;

namespace MovieServices.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly IMovieServices _movieServices;

        public MoviesController(IMovieServices movieServices)
        {
            _movieServices = movieServices;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMovies(int currentPage = 1, int pageSize = 10, string? searchTitle = null)
        {
            var response = await _movieServices.GetAllMovies(currentPage, pageSize, searchTitle, HttpContext.RequestServices.GetRequiredService<IHttpContextAccessor>());
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMovieById(Guid id)
        {
            var response = await _movieServices.GetMovieById(id);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMovie([FromForm] MovieDTO movieDTO)
        {
            var response = await _movieServices.CreateMovie(movieDTO);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMovie(Guid id, [FromBody] MovieDTO movieDTO)
        {
            var response = await _movieServices.UpdateMovie(id, movieDTO);
            return StatusCode(response.StatusCode, response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMovie(Guid id)
        {
            var response = await _movieServices.DeleteMovie(id);
            return StatusCode(response.StatusCode, response);
        }
    }
}
