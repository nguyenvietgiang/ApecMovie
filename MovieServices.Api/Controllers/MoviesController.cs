using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using MovieServices.Application.BussinessServices;
using MovieServices.Application.ModelsDTO;

namespace MovieServices.Api.Controllers
{
    [Route("v1/api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly IMovieServices _movieServices;

        public MoviesController(IMovieServices movieServices)
        {
            _movieServices = movieServices;
        }
        /// <summary>
        /// get all movie with pagging - no auth
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllMovies(int currentPage = 1, int pageSize = 10, string? searchTitle = null)
        {
            var response = await _movieServices.GetAllMovies(currentPage, pageSize, searchTitle, HttpContext.RequestServices.GetRequiredService<IHttpContextAccessor>());
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// get movie with detail by id - no auth
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMovieById(Guid id)
        {
            var response = await _movieServices.GetMovieById(id);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// create new movie - admin
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateMovie([FromForm] MovieDTO movieDTO)
        {
            var response = await _movieServices.CreateMovie(movieDTO);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// update movie infomation - admin
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMovie(Guid id, [FromForm] MovieDTO movieDTO)
        {
            var response = await _movieServices.UpdateMovie(id, movieDTO);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// delete movie - admin
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMovie(Guid id)
        {
            var response = await _movieServices.DeleteMovie(id);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// patch movie with json document (error handling) - admin
        /// </summary>
        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchMovie(Guid id, [FromBody] JsonPatchDocument<MovieDTO> patchDocument)
        {
            var response = await _movieServices.PatchMovie(id, patchDocument);
            return StatusCode(response.StatusCode, response);
        }
    }
}
