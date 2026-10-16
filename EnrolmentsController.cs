using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EventManagementAPI.Data;
using EventManagementAPI.Models;
using EventManagementAPI.Helpers;

namespace EventManagementAPI.Controllers
{
    [ApiController]
    [Route("api/participate")]
    [Route("api/enrolments")]
    public class EnrolmentsController : ControllerBase
    {
        private readonly AppDbContext _db;
        public EnrolmentsController(AppDbContext db) => _db = db;

        /// <summary>Participant enrols in event by selecting category</summary>
        [HttpPost]
        public async Task<IActionResult> Enrol(CreateEnrolmentDto dto)
        {
            if (!SessionHelper.IsAuthenticated(HttpContext.Session)) return Unauthorized();
            if (!SessionHelper.IsParticipant(HttpContext.Session) && !SessionHelper.IsOrganiser(HttpContext.Session)) return Forbid();

            var enrol = new Enrolment
            {
                UserId = SessionHelper.GetUserId(HttpContext.Session)!.Value,
                EventId = dto.EventId,
                CategoryId = dto.CategoryId,
                Status = "Registered"
            };
            _db.Enrolments.Add(enrol);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetAll), new { id = enrol.Id }, new { participate_id = enrol.Id });
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            if (!SessionHelper.IsOrganiser(HttpContext.Session)) return Forbid();
            var list = await _db.Enrolments.Include(e => e.User).Include(e => e.Event).ToListAsync();
            return Ok(list);
        }

        [HttpGet("event/{eventId}")]
        public async Task<IActionResult> GetByEvent(int eventId)
        {
            if (!SessionHelper.IsOrganiser(HttpContext.Session)) return Forbid();
            var list = await _db.Enrolments.Where(e => e.EventId == eventId).Include(e => e.User).ToListAsync();
            return Ok(list);
        }

        [HttpGet("category/{categoryId}")]
        public async Task<IActionResult> GetByCategory(int categoryId)
        {
            if (!SessionHelper.IsOrganiser(HttpContext.Session)) return Forbid();
            var list = await _db.Enrolments.Where(e => e.CategoryId == categoryId).ToListAsync();
            return Ok(list);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStatus(int id, UpdateEnrolmentStatusDto dto)
        {
            if (!SessionHelper.IsOrganiser(HttpContext.Session)) return Forbid();
            var enrol = await _db.Enrolments.FindAsync(id);
            if (enrol == null) return NotFound();
            enrol.Status = dto.Status;
            await _db.SaveChangesAsync();
            return Ok(enrol);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (!SessionHelper.IsOrganiser(HttpContext.Session)) return Forbid();
            var enrol = await _db.Enrolments.FindAsync(id);
            if (enrol == null) return NotFound();
            _db.Enrolments.Remove(enrol);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>Participant view own enrolments</summary>
        [HttpGet("my")]
        public async Task<IActionResult> MyEnrolments()
        {
            if (!SessionHelper.IsAuthenticated(HttpContext.Session)) return Unauthorized();
            var uid = SessionHelper.GetUserId(HttpContext.Session)!.Value;
            var list = await _db.Enrolments.Where(e => e.UserId == uid).Include(e => e.Event).ToListAsync();
            return Ok(list);
        }
    }
}
