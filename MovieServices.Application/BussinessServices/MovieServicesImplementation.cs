using AutoMapper;
using Minio;
using MovieServices.Application.ModelsDTO;
using MovieServices.Domain.Interfaces;
using MovieServices.Domain.Models;

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

        public async Task<IEnumerable<Movie>> GetAllMovies()
        {
            return await _movieRepository.GetAllMovies();
        }

        public async Task<Movie> GetMovieById(Guid id)
        {
            return await _movieRepository.GetMovieById(id);
        }

        public async Task<Movie> CreateMovie(MovieDTO movieDTO)
        {
            var movie = _mapper.Map<Movie>(movieDTO);
            movie.Id = Guid.NewGuid();
            movie.Status = true;

            string imageUrl = await SaveImageToMinio(movieDTO.Image);
            movie.Image = imageUrl;

            await _movieRepository.AddMovie(movie);
            return movie;
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

        public async Task UpdateMovie(Guid id, MovieDTO movieDTO)
        {
            var movie = _mapper.Map<Movie>(movieDTO);
            movie.Id = id;

            await _movieRepository.UpdateMovie(id, movie);
        }

        public async Task DeleteMovie(Guid id)
        {
            await _movieRepository.DeleteMovie(id);
        }
    }
}
