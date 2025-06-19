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
    public class AssessmentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AssessmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Assessments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AssessmentReadDTO>>> GetAssessments()
        {
            var assessments = await _context.Assessments.Include(a => a.Course).ToListAsync();

            var dtoList = assessments.Select(a => new AssessmentReadDTO
            {
                AssessmentId = a.AssessmentId,
                CourseId = a.CourseId,
                Title = a.Title,
                Questions = a.Questions,
                MaxScore = a.MaxScore,
                CourseName = a.Course?.Title
            });

            return Ok(dtoList);
        }

        // GET: api/Assessments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AssessmentReadDTO>> GetAssessment(Guid id)
        {
            var assessment = await _context.Assessments
                .Include(a => a.Course)
                .FirstOrDefaultAsync(a => a.AssessmentId == id);

            if (assessment == null)
                return NotFound();

            var dto = new AssessmentReadDTO
            {
                AssessmentId = assessment.AssessmentId,
                CourseId = assessment.CourseId,
                Title = assessment.Title,
                Questions = assessment.Questions,
                MaxScore = assessment.MaxScore,
                CourseName = assessment.Course?.Title
            };

            return Ok(dto);
        }

        // GET: api/Assessments/ByCourse/5
        [HttpGet("ByCourse/{courseId}")]
        public async Task<ActionResult<IEnumerable<AssessmentSummaryDTO>>> GetAssessmentsByCourse(Guid courseId)
        {
            var assessments = await _context.Assessments
                .Where(a => a.CourseId == courseId)
                .ToListAsync();

            var dtoList = assessments.Select(a => new AssessmentSummaryDTO
            {
                AssessmentId = a.AssessmentId,
                Title = a.Title
            });

            return Ok(dtoList);
        }

        // POST: api/Assessments
        [HttpPost]
        [Authorize(Roles = "Instructor")]
        public async Task<ActionResult<AssessmentReadDTO>> CreateAssessment(AssessmentCreateDTO dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            var course = await _context.Courses.FindAsync(dto.CourseId);
            if (course == null) return NotFound("Course not found");
            if (course.InstructorId.ToString() != userId) return Forbid();

            var assessment = new Assessment
            {
                AssessmentId = Guid.NewGuid(),
                CourseId = dto.CourseId,
                Title = dto.Title,
                Questions = dto.Questions,
                MaxScore = dto.MaxScore
            };

            _context.Assessments.Add(assessment);
            await _context.SaveChangesAsync();

            var resultDto = new AssessmentReadDTO
            {
                AssessmentId = assessment.AssessmentId,
                CourseId = assessment.CourseId,
                Title = assessment.Title,
                Questions = assessment.Questions,
                MaxScore = assessment.MaxScore,
                CourseName = course.Title
            };

            return CreatedAtAction(nameof(GetAssessment), new { id = assessment.AssessmentId }, resultDto);
        }

        // PUT: api/Assessments/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> UpdateAssessment(Guid id, AssessmentUpdateDTO dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            var existing = await _context.Assessments.Include(a => a.Course)
                .FirstOrDefaultAsync(a => a.AssessmentId == id);

            if (existing == null) return NotFound();
            if (existing.Course.InstructorId.ToString() != userId) return Forbid();

            existing.Title = dto.Title;
            existing.Questions = dto.Questions;
            existing.MaxScore = dto.MaxScore;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/Assessments/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> DeleteAssessment(Guid id)
        {
            var assessment = await _context.Assessments
                .Include(a => a.Course)
                .FirstOrDefaultAsync(a => a.AssessmentId == id);

            if (assessment == null) return NotFound();

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null || assessment.Course.InstructorId.ToString() != userId) return Forbid();

            _context.Assessments.Remove(assessment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AssessmentExists(Guid id)
        {
            return _context.Assessments.Any(e => e.AssessmentId == id);
        }
    }
}
