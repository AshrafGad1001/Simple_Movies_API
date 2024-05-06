using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MoviesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        private new List<string> _allowedExtenstions = new List<string> { ".jpg", ".png" };
        private long _MaxAllowedPosterSize = 1048576;

        public MoviesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var movies = await _context.Movies.Include(G => G.Genre).ToListAsync();

            return Ok(movies);
        }






        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromForm] MovieDTO dto)
        {
            if (!_allowedExtenstions.Contains(Path.GetExtension(dto.Poster.FileName).ToLower()))
            {
                return BadRequest("Only Png & Jpg are Allowed Extenstions . ");
            }
            if (dto.Poster.Length > _MaxAllowedPosterSize)
            {
                return BadRequest("Max Allowed Poster Size 1 MB !");
            }
            var IsVailedGenre = await _context.Genres.AnyAsync(g => g.Id == dto.GenreId);


            using var dataStream = new MemoryStream();
            await dto.Poster.CopyToAsync(dataStream);
            var Movie = new Movie
            {
                GenreId = dto.GenreId,
                Title = dto.Title,
                Poster = dataStream.ToArray(),
                Rate = dto.Rate,
                StoryLine = dto.StoryLine,
                year = dto.year
            };
            await _context.Movies.AddAsync(Movie);
            _context.SaveChanges();

            return Ok(Movie);
        }
    }
}
