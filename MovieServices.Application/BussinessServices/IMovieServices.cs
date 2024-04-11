using ApecMovieCore.BaseResponse;
using ApecMovieCore.Pagging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using MovieServices.Application.ModelsDTO;
using MovieServices.Domain.Models;

namespace MovieServices.Application.BussinessServices
{
    public interface IMovieServices
    {
        Task<Response<PaggingCore<Movie>>> GetAllMovies(int currentPage, int pageSize, string searchTitle, IHttpContextAccessor httpContextAccessor);
        Task<Response<Movie>> GetMovieById(Guid id);
        Task<Response<Movie>> CreateMovie(MovieDTO movieDTO);
        Task<Response<bool>> UpdateMovie(Guid id, MovieDTO movieDTO);
        Task<Response<bool>> DeleteMovie(Guid id);
        Task<Response<Movie>> PatchMovie(Guid id, JsonPatchDocument<MovieDTO> patchDocument);
    }
}
