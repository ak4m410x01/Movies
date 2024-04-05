using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Movies.API.Dtos;
using Movies.API.Models;
using Movies.API.Services;

namespace Movies.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        public MoviesController(IMoviesService moviesService, IGenresService genresService)
        {
            _moviesService = moviesService;
            _genresService = genresService;
        }


        private readonly IMoviesService _moviesService;
        private readonly IGenresService _genresService;


        private List<string> _allowedPosterExtensions = new List<string>() { ".jpg", ".png" };
        private long _maxAllowedPosterSize = 1_048_576;


        [HttpGet]
        public async Task<IActionResult> GetAllAsync(byte? genreId = null)
        {
            var movies = await _moviesService.GetAll(genreId);
            return Ok(movies);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromForm] MovieDto dto)
        {
            if (!_allowedPosterExtensions.Contains(Path.GetExtension(dto.Poster.FileName).ToLower()))
            {
                return BadRequest(new { Poster = "Only .jpg or .png images are allowed!" });
            }
            if (dto.Poster.Length > _maxAllowedPosterSize)
            {
                return BadRequest(new { Poster = "Max allowed size for poster is 1MB!" });
            }

            bool isValidGenre = await _genresService.IsValid(dto.GenreId);
            if (!isValidGenre)
            {
                return BadRequest(new { GenreId = "Invalid GenreId" });
            }

            using MemoryStream stream = new MemoryStream();

            await dto.Poster.CopyToAsync(stream);

            Movie movie = new Movie()
            {
                Title = dto.Title,
                Year = dto.Year,
                Rate = dto.Rate,
                Storeline = dto.Storeline,
                Poster = stream.ToArray(),
                GenreId = dto.GenreId,
            };

            await _moviesService.Add(movie);
            return Ok(movie);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            Movie? movie = await _moviesService.GetById(id);

            if (movie is null)
                return NotFound(new { error = $"Not Found movies with Id = {id}" });

            var result = new
            {
                movie.Id,
                movie.Title,
                movie.Year,
                movie.Rate,
                movie.Storeline,
                Genre = new
                {
                    movie.Genre.Id,
                    movie.Genre.Name
                },
                movie.Poster,
            };

            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromForm] MovieDto dto)
        {
            Movie? movie = await _moviesService.GetById(id);
            if (movie is null)
                return NotFound(new { error = $"Not Found Moives with Id = {id}" });

            if (dto.Title != null)
                movie.Title = dto.Title;

            if (dto.Year != null)
                movie.Year = dto.Year;

            if (dto.Rate != null)
                movie.Rate = dto.Rate;

            if (dto.Storeline != null)
                movie.Storeline = dto.Storeline;

            if (dto.Poster != null)
            {
                if (!_allowedPosterExtensions.Contains(Path.GetExtension(dto.Poster.FileName).ToLower()))
                {
                    return BadRequest(new { Poster = "Only .jpg or .png images are allowed!" });
                }
                if (dto.Poster.Length > _maxAllowedPosterSize)
                {
                    return BadRequest(new { Poster = "Max allowed size for poster is 1MB!" });
                }

                using MemoryStream stream = new MemoryStream();
                await dto.Poster.CopyToAsync(stream);

                movie.Poster = stream.ToArray();
            }

            if (dto.GenreId != null)
            {
                bool isValidGenre = await _genresService.IsValid(dto.GenreId);
                if (!isValidGenre)
                {
                    return BadRequest(new { GenreId = "Invalid GenreId" });
                }

                movie.GenreId = dto.GenreId;
            }

            return Ok(movie);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            Movie? movie = await _moviesService.GetById(id);

            if (movie is null)
                return NotFound(new { error = $"Not Found movies with Id = {id}" });

            _moviesService.Delete(movie);
            return NoContent();
        }
    }
}
