using ApecMovieCore.BaseResponse;
using ApecMovieCore.Pagging;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Minio;
using MovieServices.Application.ModelsDTO;
using MovieServices.Domain.Interfaces;
using MovieServices.Domain.Models;
using static StackExchange.Redis.Role;

namespace MovieServices.Application.BussinessServices
{
    public class MovieServicesImplementation : IMovieServices
    {
        private readonly IMovieRepository _movieRepository;
        private readonly IMapper _mapper;
        private readonly MinioClient _minioClient;

        public MovieServicesImplementation(IMovieRepository movieRepository, IMapper mapper, MinioClient minioClient)
        {
            _movieRepository = movieRepository;
            _mapper = mapper;
            _minioClient = minioClient;
        }

        public async Task<Response<PaggingCore<Movie>>> GetAllMovies(int currentPage, int pageSize, string searchTitle, IHttpContextAccessor httpContextAccessor)
        {
            var movies = await _movieRepository.GetAllMovies();

            // Lọc danh sách phim nếu có searchTitle
            if (!string.IsNullOrEmpty(searchTitle))
            {
                movies = movies.Where(m => m.Title.Contains(searchTitle, StringComparison.OrdinalIgnoreCase));
            }

            var totalRecords = movies.Count();
            var content = movies.Skip((currentPage - 1) * pageSize).Take(pageSize);
            var pagedMovies = new PaggingCore<Movie>(content, totalRecords, currentPage, pageSize, httpContextAccessor, "/api/movies");

            return new Response<PaggingCore<Movie>>(200, "Success", pagedMovies);
        }

        public async Task<Response<Movie>> GetMovieById(Guid id)
        {
            var movie = await _movieRepository.GetMovieById(id);
            return movie != null
                ? new Response<Movie>(200, "Success", movie)
                : new Response<Movie>(404, "Movie not found", null);
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
            return new Response<Movie>(200, "Movie created successfully", movie);
        }

        public async Task<Response<bool>> UpdateMovie(Guid id, MovieDTO movieDTO)
        {
            var movie = _mapper.Map<Movie>(movieDTO);
            movie.Id = id;

            try
            {
                await _movieRepository.UpdateMovie(id, movie);
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
                return new Response<bool>(200, "Movie deleted successfully", true);
            }
            catch
            {
                return new Response<bool>(404, "Movie not found", false);
            }
        }

        private async Task<string> SaveImageToMinio(Microsoft.AspNetCore.Http.IFormFile image)
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
            var moviePatchDocument = _mapper.Map<JsonPatchDocument<Movie>>(patchDocument);
            var patchedMovie = await _movieRepository.PatchMovie(id, moviePatchDocument);

            return new Response<Movie>(200, "Movie patched successfully", patchedMovie);
        }

    }
}
