using EduSyncAPI.Data;
using EduSyncAPI.DTOs;
using EduSyncAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EduSyncAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("Profile")]
        public async Task<ActionResult<UserReadDTO>> GetProfile()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserId.ToString() == userId);

            if (user == null) return NotFound();

            return new UserReadDTO
            {
                UserId = user.UserId,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role
            };
        }

        [HttpGet("Instructors")]
        public async Task<ActionResult<IEnumerable<UserReadDTO>>> GetInstructors()
        {
            return await _context.Users
                .Where(u => u.Role == "Instructor")
                .Select(u => new UserReadDTO
                {
                    UserId = u.UserId,
                    Name = u.Name,
                    Email = u.Email,
                    Role = u.Role
                }).ToListAsync();
        }

        [HttpGet("Students")]
        [Authorize(Roles = "Instructor")]
        public async Task<ActionResult<IEnumerable<UserReadDTO>>> GetStudents()
        {
            return await _context.Users
                .Where(u => u.Role == "Student")
                .Select(u => new UserReadDTO
                {
                    UserId = u.UserId,
                    Name = u.Name,
                    Email = u.Email,
                    Role = u.Role
                }).ToListAsync();
        }

        [HttpPut("Profile")]
        public async Task<IActionResult> UpdateProfile(UserUpdateDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            var user = await _context.Users.FindAsync(Guid.Parse(userId));
            if (user == null) return NotFound();

            user.Name = dto.Name;
            if (!string.IsNullOrEmpty(dto.Password))
            {
                // Assume password is hashed elsewhere if needed
                user.PasswordHash = dto.Password;
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
