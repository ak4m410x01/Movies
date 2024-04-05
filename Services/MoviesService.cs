using Microsoft.EntityFrameworkCore;
using Movies.API.Models;

namespace Movies.API.Services
{
    public class MoviesService : IMoviesService
    {
        public MoviesService(ApplicationDbContext context)
        {
            _context = context;
        }

        private readonly ApplicationDbContext _context;

        public async Task<IEnumerable<object>> GetAll(byte? genreId = null)
        {
            return await _context.Movies
                .Where(m => m.GenreId == genreId || genreId == null)
                .Include(m => m.Genre)
                .OrderByDescending(m => m.Rate)
                .Select(m => new
                {
                    m.Id,
                    m.Title,
                    m.Year,
                    m.Rate,
                    m.Storeline,
                    Genre = new
                    {
                        m.Genre.Id,
                        m.Genre.Name
                    },
                    m.Poster,
                })
                .ToListAsync();
        }

        public async Task<Movie?> GetById(int id)
        {
            return await _context.Movies
                .Include(m => m.Genre)
                .SingleOrDefaultAsync(m => m.Id == id);
        }

        public async Task<Movie> Add(Movie movie)
        {
            await _context.Movies.AddAsync(movie);
            _context.SaveChanges();
            return movie;
        }

        public Movie Update(Movie movie)
        {
            _context.Movies.Update(movie);
            _context.SaveChanges();
            return movie;
        }

        public Movie Delete(Movie movie)
        {
            _context.Movies.Remove(movie);
            _context.SaveChanges();
            return movie;
        }
    }
}
