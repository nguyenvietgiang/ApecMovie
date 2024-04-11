using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using MovieServices.Domain.Interfaces;
using MovieServices.Domain.Models;
using MovieServices.Infrastructure.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieServices.Infrastructure.Repository
{
    public class MovieRepository : IMovieRepository
    {
        private readonly MovieDbConext _dbContext;

        public MovieRepository(MovieDbConext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Movie>> GetAllMovies()
        {
            return await _dbContext.Movies.AsNoTracking().ToListAsync();
        }

        public async Task<Movie> GetMovieById(Guid id)
        {
            return await _dbContext.Movies.FindAsync(id);
        }

        public async Task AddMovie(Movie movie)
        {
            _dbContext.Movies.Add(movie);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateMovie(Guid id, Movie movie)
        {
            var existingMovie = await _dbContext.Movies.FindAsync(id);
            if (existingMovie == null)
                throw new Exception("Movie not found");

            existingMovie.Title = movie.Title;
            existingMovie.Category = movie.Category;
            existingMovie.Description = movie.Description;
            existingMovie.Image = movie.Image;
            existingMovie.Director = movie.Director;

            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteMovie(Guid id)
        {
            var movieToDelete = await _dbContext.Movies.FindAsync(id);
            if (movieToDelete == null)
                throw new Exception("Movie not found");

            _dbContext.Movies.Remove(movieToDelete);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Movie> PatchMovie(Guid id, JsonPatchDocument<Movie> patchDocument)
        {
            var movie = await _dbContext.Movies.FindAsync(id);
            if (movie == null)
            {
                return null;
            }

            patchDocument.ApplyTo(movie);
            await _dbContext.SaveChangesAsync();

            return movie;
        }
    }
}
