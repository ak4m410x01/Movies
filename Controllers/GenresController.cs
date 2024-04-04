using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Movies.API.Dtos;
using Movies.API.Models;

namespace Movies.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenresController : ControllerBase
    {
        public GenresController(ApplicationDbContext context)
        {
            _context = context;
        }

        private readonly ApplicationDbContext _context;

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            List<Genre> genres = await _context.Genres.OrderBy(g => g.Name).ToListAsync();
            return Ok(genres);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(GenreDto dto)
        {
            Genre genre = new Genre() { Name = dto.Name };

            await _context.Genres.AddAsync(genre);
            await _context.SaveChangesAsync();
            return Ok(genre);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(int id)
        {
            Genre? genre = await _context.Genres.SingleOrDefaultAsync(g => g.Id == id);
            if (genre is null)
            {
                return NotFound(new { error = $"No Genre was found with Id = {id}" });
            }

            return Ok(genre);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] GenreDto dto)
        {
            Genre? genre = await _context.Genres.SingleOrDefaultAsync(g => g.Id == id);
            if (genre is null)
            {
                return NotFound(new { error = $"No Genre was found with Id = {id}" });
            }

            genre.Name = dto.Name;
            await _context.SaveChangesAsync();
            return Ok(genre);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            Genre? genre = await _context.Genres.SingleOrDefaultAsync(g => g.Id == id);
            if (genre is null)
            {
                return NotFound(new { error = $"No Genre was found with Id = {id}" });
            }

            _context.Genres.Remove(genre);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
