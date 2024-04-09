using MovieServices.Application.ModelsDTO;
using MovieServices.Domain.Models;

namespace MovieServices.Application.BussinessServices
{
    public interface IMovieServices
    {
        Task<IEnumerable<Movie>> GetAllMovies();
        Task<Movie> GetMovieById(Guid id);
        Task<Movie> CreateMovie(MovieDTO movieDTO);
        Task UpdateMovie(Guid id, MovieDTO movieDTO);
        Task DeleteMovie(Guid id);
    }
}
