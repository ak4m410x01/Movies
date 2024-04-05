using Movies.API.Models;

namespace Movies.API.Services
{
    public interface IMoviesService
    {
        Task<IEnumerable<object>> GetAll(byte? genreId = null);
        Task<Movie?> GetById(int id);
        Task<Movie> Add(Movie movie);
        Movie Update(Movie movie);
        Movie Delete(Movie movie);
    }
}
