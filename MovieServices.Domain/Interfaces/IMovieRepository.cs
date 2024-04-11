using Microsoft.AspNetCore.JsonPatch;
using MovieServices.Domain.Models;

namespace MovieServices.Domain.Interfaces
{
    public interface IMovieRepository
    {
        Task<IEnumerable<Movie>> GetAllMovies();
        Task<Movie> GetMovieById(Guid id);
        Task AddMovie(Movie movie);
        Task UpdateMovie(Guid id, Movie movie);
        Task DeleteMovie(Guid id);
        Task<Movie> PatchMovie(Guid id, JsonPatchDocument<Movie> patchDocument);
    }
}
