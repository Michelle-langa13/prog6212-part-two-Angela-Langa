using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EventManagementAPI.Data;
using EventManagementAPI.Models;
using EventManagementAPI.Helpers;

namespace EventManagementAPI.Controllers
{
    [ApiController]
    [Route("api/venues")]
    public class VenuesController : ControllerBase
    {
        private readonly AppDbContext _db;
        public VenuesController(AppDbContext db) => _db = db;

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _db.Venues.ToListAsync());

        [HttpPost]
        public async Task<IActionResult> Create(CreateVenueDto dto)
        {
            if (!SessionHelper.IsOrganiser(HttpContext.Session)) return Forbid();
            var venue = new Venue { Name = dto.Name, Address = dto.Address, City = dto.City, Capacity = dto.Capacity };
            _db.Venues.Add(venue);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetAll), new { id = venue.Id }, venue);
        }
    }
}
