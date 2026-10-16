using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EventManagementAPI.Data;
using EventManagementAPI.Models;
using EventManagementAPI.Helpers;

namespace EventManagementAPI.Controllers
{
    [ApiController].
    [Route("api/user")]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _db;
        public UserController(AppDbContext db) => _db = db;

        private int? CurrentUserId() => SessionHelper.GetUserId(HttpContext.Session);

        /// <summary>Get own profile - both roles</summary>
        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            if (!SessionHelper.IsAuthenticated(HttpContext.Session)) return Unauthorized();
            var id = CurrentUserId();
            var user = await _db.Users.FindAsync(id);
            if (user == null) return NotFound();
            return Ok(new { user.Id, user.FirstName, user.LastName, user.Email, role = user.Role.ToString() });
        }

        /// <summary>Update own profile - both roles</summary>
        [HttpPut("me")]
        public async Task<IActionResult> UpdateMe(UpdateProfileDto dto)
        {
            if (!SessionHelper.IsAuthenticated(HttpContext.Session)) return Unauthorized();
            var user = await _db.Users.FindAsync(CurrentUserId());
            if (user == null) return NotFound();
            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;
            await _db.SaveChangesAsync();
            return Ok(user);
        }

        /// <summary>Get all users (Organiser only) - satisfies "2 records administrators" from plan</summary>
        [HttpGet("/api/admin")]
        [HttpGet("/api/users")]
        public async Task<IActionResult> GetAll()
        {
            if (!SessionHelper.IsOrganiser(HttpContext.Session)) return Forbid();
            var users = await _db.Users.Select(u => new { u.Id, u.FirstName, u.LastName, u.Email, Role = u.Role.ToString() }).ToListAsync();
            return Ok(users);
        }
    }
}
