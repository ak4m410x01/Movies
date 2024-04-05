using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Movies.API.Dtos;
using Movies.API.Models;
using Movies.API.Services;

namespace Movies.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenresController : ControllerBase
    {
        public GenresController(IGenresService genresService)
        {
            _genresService = genresService;
        }

        private readonly IGenresService _genresService;

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            IEnumerable<Genre> genres = await _genresService.GetAll();
            return Ok(genres);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(GenreDto dto)
        {
            Genre genre = new Genre() { Name = dto.Name };
            await _genresService.Add(genre);
            return Ok(genre);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(byte id)
        {
            Genre? genre = await _genresService.GetById(id);
            if (genre is null)
                return NotFound(new { error = $"No Genre was found with Id = {id}" });

            return Ok(genre);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(byte id, [FromBody] GenreDto dto)
        {
            Genre? genre = await _genresService.GetById(id);
            if (genre is null)
                return NotFound(new { error = $"No Genre was found with Id = {id}" });

            _genresService.Update(genre);
            return Ok(genre);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(byte id)
        {
            Genre? genre = await _genresService.GetById(id);
            if (genre is null)
                return NotFound(new { error = $"No Genre was found with Id = {id}" });

            _genresService.Delete(genre);
            return NoContent();
        }
    }
}
