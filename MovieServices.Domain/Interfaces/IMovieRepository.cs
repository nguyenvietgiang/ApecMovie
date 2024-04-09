using MovieServices.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieServices.Domain.Interfaces
{
    public interface IMovieRepository
    {
        Task<IEnumerable<Movie>> GetAllMovies();
        Task<Movie> GetMovieById(Guid id);
        Task AddMovie(Movie movie);
        Task UpdateMovie(Guid id, Movie movie);
        Task DeleteMovie(Guid id);
    }
}
