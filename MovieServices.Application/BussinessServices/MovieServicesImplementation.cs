using ApecMovieCore.BaseResponse;
using ApecMovieCore.Pagging;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Caching.Distributed;
using Minio;
using MovieServices.Application.ModelsDTO;
using MovieServices.Domain.Interfaces;
using MovieServices.Domain.Models;
using System.Text.Json;
using System.Linq;

namespace MovieServices.Application.BussinessServices
{
    public class MovieServicesImplementation : IMovieServices
    {
        private readonly IMovieRepository _movieRepository;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _cache;
        private readonly MinioClient _minioClient;

        public MovieServicesImplementation(IMovieRepository movieRepository, IMapper mapper, MinioClient minioClient, IDistributedCache distributedCache)
        {
            _movieRepository = movieRepository;
            _mapper = mapper;
            _minioClient = minioClient;
            _cache = distributedCache;
        }

        public async Task<Response<PaggingCore<Movie>>> GetAllMovies(int currentPage, int pageSize, string searchTitle, IHttpContextAccessor httpContextAccessor)
        {
            string cacheKey = "AllMovies";
            var cachedMovies = await _cache.GetStringAsync(cacheKey);
            IEnumerable<Movie> movies;

            if (!string.IsNullOrEmpty(cachedMovies))
            {
                movies = JsonSerializer.Deserialize<IEnumerable<Movie>>(cachedMovies);
            }
            else
            {
                movies = await _movieRepository.GetAllMovies();
                await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(movies), new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                });
            }

            if (!string.IsNullOrEmpty(searchTitle))
            {
                movies = movies.Where(m => m.Title.Contains(searchTitle, StringComparison.OrdinalIgnoreCase));
            }

            var totalRecords = movies.Count();
            var content = movies.Skip((currentPage - 1) * pageSize).Take(pageSize);
            var pagedMoviesResult = new PaggingCore<Movie>(content, totalRecords, currentPage, pageSize, httpContextAccessor, "/v1/api/movies");

            return new Response<PaggingCore<Movie>>(200, "Success", pagedMoviesResult);
        }

        public async Task<Response<Movie>> GetMovieById(Guid id)
        {
            string cacheKey = $"GetMovieById_{id}";
            var cachedMovie = await _cache.GetStringAsync(cacheKey);

            if (!string.IsNullOrEmpty(cachedMovie))
            {
                var movie = JsonSerializer.Deserialize<Movie>(cachedMovie);
                return new Response<Movie>(200, "Success", movie);
            }

            var movieFromDb = await _movieRepository.GetMovieById(id);
            if (movieFromDb != null)
            {
                await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(movieFromDb), new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                });
                return new Response<Movie>(200, "Success", movieFromDb);
            }

            return new Response<Movie>(404, "Movie not found", null);
        }

        public async Task<Response<Movie>> CreateMovie(MovieDTO movieDTO)
        {
            var movie = _mapper.Map<Movie>(movieDTO);
            movie.Id = Guid.NewGuid();
            movie.Status = true;

            string imageUrl = await SaveImageToMinio(movieDTO.Image);
            if (string.IsNullOrEmpty(imageUrl))
            {
                return new Response<Movie>(500, "Failed to save image", null);
            }

            movie.Image = imageUrl;

            await _movieRepository.AddMovie(movie);

            // Invalidate cache
            await _cache.RemoveAsync("AllMovies");

            return new Response<Movie>(200, "Movie created successfully", movie);
        }

        public async Task<Response<bool>> UpdateMovie(Guid id, MovieDTO movieDTO)
        {
            var movie = _mapper.Map<Movie>(movieDTO);
            movie.Id = id;

            try
            {
                await _movieRepository.UpdateMovie(id, movie);

                // Invalidate cache
                await _cache.RemoveAsync("AllMovies");
                await _cache.RemoveAsync($"GetMovieById_{id}");

                return new Response<bool>(200, "Movie updated successfully", true);
            }
            catch
            {
                return new Response<bool>(404, "Movie not found", false);
            }
        }

        public async Task<Response<bool>> DeleteMovie(Guid id)
        {
            try
            {
                await _movieRepository.DeleteMovie(id);

                // Invalidate cache
                await _cache.RemoveAsync("AllMovies");
                await _cache.RemoveAsync($"GetMovieById_{id}");

                return new Response<bool>(200, "Movie deleted successfully", true);
            }
            catch
            {
                return new Response<bool>(404, "Movie not found", false);
            }
        }

        private async Task<string> SaveImageToMinio(IFormFile image)
        {
            if (image != null && image.Length > 0)
            {
                var imageName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
                var bucketName = "apecmovie";

                try
                {
                    var bucketExists = await _minioClient.BucketExistsAsync(bucketName);
                    if (!bucketExists)
                    {
                        await _minioClient.MakeBucketAsync(bucketName);
                    }

                    await _minioClient.PutObjectAsync(bucketName, imageName, image.OpenReadStream(), image.Length);
                    return $"localhost:9000/{bucketName}/{imageName}";
                }
                catch (Minio.Exceptions.MinioException ex)
                {
                    // Handle exception
                    return null;
                }
            }

            return null;
        }

        public async Task<Response<Movie>> PatchMovie(Guid id, JsonPatchDocument<MovieDTO> patchDocument)
        {
            var movie = await _movieRepository.GetMovieById(id);

            if (movie == null)
            {
                return new Response<Movie>(404, "Movie not found", null);
            }

            var movieDTO = _mapper.Map<MovieDTO>(movie);
            patchDocument.ApplyTo(movieDTO);
            var updatedMovie = _mapper.Map<Movie>(movieDTO);

            try
            {
                await _movieRepository.UpdateMovie(id, updatedMovie);

                // Invalidate cache
                await _cache.RemoveAsync("AllMovies");
                await _cache.RemoveAsync($"GetMovieById_{id}");

                return new Response<Movie>(200, "Movie patched successfully", updatedMovie);
            }
            catch
            {
                return new Response<Movie>(404, "Movie not found", null);
            }
        }
    }
}

