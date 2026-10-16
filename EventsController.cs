using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EventManagementAPI.Data;
using EventManagementAPI.Models;
using EventManagementAPI.Helpers;

namespace EventManagementAPI.Controllers
{
    [ApiController]
    [Route("api/events")]
    public class EventsController : ControllerBase
    {
        private readonly AppDbContext _db;
        public EventsController(AppDbContext db) => _db = db;

        /// <summary>Get all events - public for Participants, Organisers</summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var events = await _db.Events.Include(e => e.Category).Include(e => e.Venue).ToListAsync();
            return Ok(events);
        }

        /// <summary>Get event by ID</summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var ev = await _db.Events.Include(e => e.Category).Include(e => e.Venue).FirstOrDefaultAsync(e => e.Id == id);
            if (ev == null) return NotFound();
            return Ok(ev);
        }

        /// <summary>Create new event - Organiser only</summary>
        [HttpPost]
        public async Task<IActionResult> Create(CreateEventDto dto)
        {
            if (!SessionHelper.IsOrganiser(HttpContext.Session)) return Forbid();
            if (!new[] { "run", "walk", "cycle" }.Contains(dto.EventType.ToLower()))
                return BadRequest("EventType must be run, walk, or cycle");

            var ev = new Event
            {
                Name = dto.Name,
                Description = dto.Description,
                EventDate = dto.EventDate,
                Location = dto.Location,
                Distance = dto.Distance,
                EventType = dto.EventType.ToLower(),
                CategoryId = dto.CategoryId,
                VenueId = dto.VenueId,
                CreatedByUserId = SessionHelper.GetUserId(HttpContext.Session)!.Value
            };
            _db.Events.Add(ev);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = ev.Id }, new { event_id = ev.Id });
        }

        /// <summary>Update event - Organiser only</summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateEventDto dto)
        {
            if (!SessionHelper.IsOrganiser(HttpContext.Session)) return Forbid();
            var ev = await _db.Events.FindAsync(id);
            if (ev == null) return NotFound();
            ev.Name = dto.Name;
            ev.Description = dto.Description;
            ev.EventDate = dto.EventDate;
            ev.Location = dto.Location;
            ev.Distance = dto.Distance;
            ev.EventType = dto.EventType;
            ev.CategoryId = dto.CategoryId;
            ev.VenueId = dto.VenueId;
            await _db.SaveChangesAsync();
            return Ok(ev);
        }

        /// <summary>Delete event - Organiser only</summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (!SessionHelper.IsOrganiser(HttpContext.Session)) return Forbid();
            var ev = await _db.Events.FindAsync(id);
            if (ev == null) return NotFound();
            _db.Events.Remove(ev);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
