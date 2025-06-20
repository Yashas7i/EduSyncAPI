using EduSyncAPI.Data;
using EduSyncAPI.DTOs;
using EduSyncAPI.Models;
using EduSyncAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EduSyncAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CoursesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IBlobService _blobService;

        public CoursesController(ApplicationDbContext context, IBlobService blobService)
        {
            _context = context;
            _blobService = blobService;
        }

        // GET: api/Courses
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CourseReadDTO>>> GetCourses()
        {
            var courses = await _context.Courses
                .Include(c => c.Instructor)
                .Include(c => c.Assessments)
                .ToListAsync();

            var courseDtos = courses.Select(c => new CourseReadDTO
            {
                CourseId = c.CourseId,
                Title = c.Title,
                Description = c.Description,
                MediaUrl = c.MediaUrl,
                InstructorId = c.InstructorId,
                InstructorName = c.Instructor?.Name,
                Assessments = c.Assessments.Select(a => new AssessmentSummaryDTO
                {
                    AssessmentId = a.AssessmentId,
                    Title = a.Title
                }).ToList()
            });

            return Ok(courseDtos);
        }

        // GET: api/Courses/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CourseReadDTO>> GetCourse(Guid id)
        {
            var course = await _context.Courses
                .Include(c => c.Instructor)
                .Include(c => c.Assessments)
                .FirstOrDefaultAsync(c => c.CourseId == id);

            if (course == null)
                return NotFound();

            var dto = new CourseReadDTO
            {
                CourseId = course.CourseId,
                Title = course.Title,
                Description = course.Description,
                MediaUrl = course.MediaUrl,
                InstructorId = course.InstructorId,
                InstructorName = course.Instructor?.Name,
                Assessments = course.Assessments.Select(a => new AssessmentSummaryDTO
                {
                    AssessmentId = a.AssessmentId,
                    Title = a.Title
                }).ToList()
            };

            return Ok(dto);
        }

        // POST: api/Courses
        [HttpPost]
        [Authorize(Roles = "Instructor,Admin")]
        public async Task<ActionResult<CourseReadDTO>> CreateCourse(CourseCreateDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            var course = new Course
            {
                CourseId = Guid.NewGuid(),
                Title = dto.Title,
                Description = dto.Description,
                MediaUrl = dto.MediaUrl,
                InstructorId = Guid.Parse(userId)
            };

            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            var instructor = await _context.Users.FindAsync(course.InstructorId);

            var readDto = new CourseReadDTO
            {
                CourseId = course.CourseId,
                Title = course.Title,
                Description = course.Description,
                MediaUrl = course.MediaUrl,
                InstructorId = course.InstructorId,
                InstructorName = instructor?.Name,
                Assessments = new List<AssessmentSummaryDTO>()
            };

            return CreatedAtAction(nameof(GetCourse), new { id = course.CourseId }, readDto);
        }

        // PUT: api/Courses/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Instructor,Admin")]
        public async Task<IActionResult> UpdateCourse(Guid id, CourseUpdateDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            var course = await _context.Courses.FindAsync(id);
            if (course == null) return NotFound();

            if (course.InstructorId.ToString() != userId)
                return Forbid();

            course.Title = dto.Title;
            course.Description = dto.Description;
            course.MediaUrl = dto.MediaUrl;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/Courses/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Instructor,Admin")]
        public async Task<IActionResult> DeleteCourse(Guid id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null) return NotFound();

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null || course.InstructorId.ToString() != userId)
                return Forbid();

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/Courses/AddCourse with File Upload
        [HttpPost("AddCourse")]
        [Authorize(Roles = "Instructor,Admin")]
        public async Task<IActionResult> AddCourse([FromForm] CourseDto courseDto, IFormFile mediaFile)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (mediaFile == null || mediaFile.Length == 0)
                return BadRequest("Media file is required.");

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            var mediaUrl = await _blobService.UploadFileAsync(mediaFile);

            var course = new Course
            {
                CourseId = Guid.NewGuid(),
                Title = courseDto.Title,
                Description = courseDto.Description,
                MediaUrl = mediaUrl,
                InstructorId = Guid.Parse(userId)
            };

            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            return Ok(course);
        }
    }
}
