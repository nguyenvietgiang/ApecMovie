using ApecMoviePortal.Models;

namespace ApecMoviePortal.Services.MovieServices 
{
    public interface IMovieService
    {
        Task<PaginatedResponse<Movie>> GetMoviesAsync();
    }
}
