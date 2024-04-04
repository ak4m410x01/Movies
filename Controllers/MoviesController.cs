using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop.Infrastructure;
using Movies.API.Dtos;
using Movies.API.Models;

namespace Movies.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        public MoviesController(ApplicationDbContext context)
        {
            _context = context;
        }

        private readonly ApplicationDbContext _context;

        private List<string> _allowedPosterExtensions = new List<string>() { ".jpg", ".png" };
        private long _maxAllowedPosterSize = 1_048_576;

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

            bool isValidGenre = await _context.Genres.AnyAsync(g => g.Id == dto.GenreId);
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

            await _context.Movies.AddAsync(movie);
            await _context.SaveChangesAsync();
            return Ok(movie);
        }
    }
}
