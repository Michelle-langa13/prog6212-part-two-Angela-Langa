using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EventManagementAPI.Data;
using EventManagementAPI.Models;
using EventManagementAPI.Helpers;

namespace EventManagementAPI.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _db;
        public AuthController(AppDbContext db) => _db = db;

        /// <summary>Register a new user as Organiser or Participant</summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            if (await _db.Users.AnyAsync(u => u.Email == dto.Email))
                return BadRequest("Email already registered");

            if (!Enum.TryParse<UserRole>(dto.Role, true, out var role))
                return BadRequest("Role must be Organiser or Participant");

            // Enforce POE roles
            if (role != UserRole.Organiser && role != UserRole.Participant && role != UserRole.Admin)
                return BadRequest("Invalid role");

            var user = new User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = role
            };
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(Register), new { id = user.Id }, new { user.Id, user.Email, user.Role });
        }

        /// <summary>Login and create session</summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return Unauthorized("Invalid credentials");

            SessionHelper.SetUserSession(HttpContext.Session, user.Id, user.Role.ToString(), user.Email);
            return Ok(new { token = HttpContext.Session.Id, user.Id, user.Email, role = user.Role.ToString(), message = "Session created" });
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return Ok("Logged out");
        }
    }
}
