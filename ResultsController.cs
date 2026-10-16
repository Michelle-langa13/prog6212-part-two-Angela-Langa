using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EventManagementAPI.Data;
using EventManagementAPI.Models;
using EventManagementAPI.Helpers;

namespace EventManagementAPI.Controllers
{
    [ApiController]
    [Route("api/results")]
    public class ResultsController : ControllerBase
    {
        private readonly AppDbContext _db;
        public ResultsController(AppDbContext db) => _db = db;

        /// <summary>Add result for participant - Organiser only</summary>
        [HttpPost]
        public async Task<IActionResult> Create(CreateResultDto dto)
        {
            if (!SessionHelper.IsOrganiser(HttpContext.Session)) return Forbid();
            if (!TimeSpan.TryParse(dto.FinishTime, out var ft))
                return BadRequest("FinishTime must be hh:mm:ss");

            var result = new Result { EnrolmentId = dto.EnrolmentId, FinishTime = ft, Position = dto.Position, PointsEarned = dto.PointsEarned };
            _db.Results.Add(result);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetByEvent), new { id = result.Id }, new { results_id = result.Id });
        }

        [HttpGet("event/{eventId}")]
        public async Task<IActionResult> GetByEvent(int eventId)
        {
            var results = await _db.Results.Include(r => r.Enrolment).Where(r => r.Enrolment!.EventId == eventId).ToListAsync();
            return Ok(results);
        }

        [HttpGet("participate/{participateId}")]
        public async Task<IActionResult> GetByParticipant(int participateId)
        {
            // Participant can view own, Organiser can view any
            if (!SessionHelper.IsAuthenticated(HttpContext.Session)) return Unauthorized();
            var role = SessionHelper.GetUserRole(HttpContext.Session);
            var uid = SessionHelper.GetUserId(HttpContext.Session)!.Value;

            var enrol = await _db.Enrolments.FindAsync(participateId);
            if (enrol == null) return NotFound();

            if (role == "Participant" && enrol.UserId != uid) return Forbid();

            var results = await _db.Results.Where(r => r.EnrolmentId == participateId).ToListAsync();
            return Ok(results);
        }

        [HttpGet("my")]
        public async Task<IActionResult> MyResults()
        {
            if (!SessionHelper.IsAuthenticated(HttpContext.Session)) return Unauthorized();
            var uid = SessionHelper.GetUserId(HttpContext.Session)!.Value;
            var results = await _db.Results.Include(r => r.Enrolment).Where(r => r.Enrolment!.UserId == uid).ToListAsync();
            return Ok(results);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateResultDto dto)
        {
            if (!SessionHelper.IsOrganiser(HttpContext.Session)) return Forbid();
            var result = await _db.Results.FindAsync(id);
            if (result == null) return NotFound();
            result.FinishTime = TimeSpan.Parse(dto.FinishTime);
            result.Position = dto.Position;
            result.PointsEarned = dto.PointsEarned;
            await _db.SaveChangesAsync();
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (!SessionHelper.IsOrganiser(HttpContext.Session)) return Forbid();
            var result = await _db.Results.FindAsync(id);
            if (result == null) return NotFound();
            _db.Results.Remove(result);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
