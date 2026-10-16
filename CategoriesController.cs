using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EventManagementAPI.Data;
using EventManagementAPI.Models;
using EventManagementAPI.Helpers;

namespace EventManagementAPI.Controllers
{
    [ApiController]
    [Route("api/categories")]
    public class CategoriesController : ControllerBase
    {
        private readonly AppDbContext _db;
        public CategoriesController(AppDbContext db) => _db = db;

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _db.Categories.ToListAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var cat = await _db.Categories.FindAsync(id);
            return cat == null ? NotFound() : Ok(cat);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCategoryDto dto)
        {
            if (!SessionHelper.IsOrganiser(HttpContext.Session)) return Forbid();
            var cat = new Category { Name = dto.Name, AgeGroup = dto.AgeGroup, Distance = dto.Distance };
            _db.Categories.Add(cat);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = cat.Id }, new { category_id = cat.Id });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, CreateCategoryDto dto)
        {
            if (!SessionHelper.IsOrganiser(HttpContext.Session)) return Forbid();
            var cat = await _db.Categories.FindAsync(id);
            if (cat == null) return NotFound();
            cat.Name = dto.Name; cat.AgeGroup = dto.AgeGroup; cat.Distance = dto.Distance;
            await _db.SaveChangesAsync();
            return Ok(cat);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (!SessionHelper.IsOrganiser(HttpContext.Session)) return Forbid();
            var cat = await _db.Categories.FindAsync(id);
            if (cat == null) return NotFound();
            _db.Categories.Remove(cat);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>Get events in a category - shows "a category contains many events"</summary>
        [HttpGet("{id}/events")]
        public async Task<IActionResult> GetEventsInCategory(int id)
        {
            var events = await _db.Events.Where(e => e.CategoryId == id).ToListAsync();
            return Ok(events);
        }
    }
}
