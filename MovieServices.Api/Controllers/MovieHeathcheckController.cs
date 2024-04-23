using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MovieServices.Application.BussinessServices;
using MovieServices.Application.ModelsDTO;

namespace MovieServices.Api.Controllers
{
    [Route("healthcheck")]
    [ApiController]
    public class MovieHeathcheckController : ControllerBase
    {
        private readonly IMovieServices _movieServices;
        public MovieHeathcheckController(IMovieServices movieServices)
        {
          _movieServices = movieServices;
        }

        [HttpGet("movies/all")]
        public async Task<IActionResult> GetAllMoviesHealthCheck(int currentPage = 1, int pageSize = 10, string? searchTitle = null)
        {
            try
            {
                var response = await _movieServices.GetAllMovies(currentPage, pageSize, searchTitle, HttpContext.RequestServices.GetRequiredService<IHttpContextAccessor>());

                if (response.StatusCode == 200)
                {
                    return Ok(new { Status = "Healthy" });
                }
                else
                {
                    return StatusCode(response.StatusCode, response);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while checking health: {ex.Message}");
            }
        }

        [HttpGet("get/{id}")]
        public async Task<IActionResult> GetMovieByIdHealthCheck(Guid id)
        {
            try
            {
                var response = await _movieServices.GetMovieById(id);

                if (response.StatusCode == 200)
                {
                    return Ok(new { Status = "Healthy" });
                }
                else
                {
                    return StatusCode(response.StatusCode, response);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while checking health: {ex.Message}");
            }
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateMovieHealthCheck([FromForm] MovieDTO movieDTO)
        {
            try
            {
                var response = await _movieServices.CreateMovie(movieDTO);

                if (response.StatusCode == 201) // Assuming 201 is the status code for successful creation
                {
                    return Ok(new { Status = "Healthy" });
                }
                else
                {
                    return StatusCode(response.StatusCode, response);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while checking health: {ex.Message}");
            }
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateMovieHealthCheck(Guid id, [FromForm] MovieDTO movieDTO)
        {
            try
            {
                var response = await _movieServices.UpdateMovie(id, movieDTO);

                if (response.StatusCode == 200)
                {
                    return Ok(new { Status = "Healthy" });
                }
                else
                {
                    return StatusCode(response.StatusCode, response);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while checking health: {ex.Message}");
            }
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteMovieHealthCheck(Guid id)
        {
            try
            {
                var response = await _movieServices.DeleteMovie(id);

                if (response.StatusCode == 200)
                {
                    return Ok(new { Status = "Healthy" });
                }
                else
                {
                    return StatusCode(response.StatusCode, response);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while checking health: {ex.Message}");
            }
        }

    }
}
