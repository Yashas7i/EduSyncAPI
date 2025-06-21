using EduSyncAPI.Data;
using EduSyncAPI.Models;
using EduSyncAPI.DTOs;
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
    public class ResultsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly EventHubService _eventHubService;

        public ResultsController(ApplicationDbContext context, EventHubService eventHubService)
        {
            _context = context;
            _eventHubService = eventHubService;
        }

        // POST: api/Results
        [HttpPost]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<ResultReadDTO>> CreateResult(ResultCreateDTO dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            var assessment = await _context.Assessments
                .Include(a => a.Course)
                .FirstOrDefaultAsync(a => a.AssessmentId == dto.AssessmentId);

            if (assessment == null)
                return NotFound("Assessment not found");

            if (dto.Score < 0 || dto.Score > assessment.MaxScore)
                return BadRequest("Invalid score");

            var result = new Result
            {
                ResultId = Guid.NewGuid(),
                AssessmentId = dto.AssessmentId,
                UserId = Guid.Parse(userId),
                Score = dto.Score,
                AttemptDate = DateTime.UtcNow
            };

            _context.Results.Add(result);
            await _context.SaveChangesAsync();

            var user = await _context.Users.FindAsync(result.UserId);

            var resultDTO = new ResultReadDTO
            {
                ResultId = result.ResultId,
                AssessmentId = result.AssessmentId,
                UserId = result.UserId,
                Score = result.Score,
                AttemptDate = result.AttemptDate,
                AssessmentTitle = assessment.Title,
                UserName = user?.Name
            };

            // ✅ Send Event to Azure Event Hub (ResultReadDTO matches QuizResults table)
            await _eventHubService.SendEventAsync(resultDTO);

            return CreatedAtAction(nameof(GetResult), new { id = result.ResultId }, resultDTO);
        }

        // GET: api/Results/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ResultReadDTO>> GetResult(Guid id)
        {
            var result = await _context.Results
                .Include(r => r.Assessment)
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.ResultId == id);

            if (result == null) return NotFound();

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            if (role == "Student" && result.UserId.ToString() != userId) return Forbid();
            if (role == "Instructor" && result.Assessment.Course.InstructorId.ToString() != userId) return Forbid();

            var dto = new ResultReadDTO
            {
                ResultId = result.ResultId,
                AssessmentId = result.AssessmentId,
                UserId = result.UserId,
                Score = result.Score,
                AttemptDate = result.AttemptDate,
                AssessmentTitle = result.Assessment?.Title,
                UserName = result.User?.Name
            };

            return Ok(dto);
        }

        // GET: api/Results/User
        [HttpGet("User")]
        public async Task<ActionResult<IEnumerable<ResultReadDTO>>> GetUserResults()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            var results = await _context.Results
                .Include(r => r.Assessment)
                .Include(r => r.User)
                .Where(r => r.UserId.ToString() == userId)
                .ToListAsync();

            var dtoList = results.Select(r => new ResultReadDTO
            {
                ResultId = r.ResultId,
                AssessmentId = r.AssessmentId,
                UserId = r.UserId,
                Score = r.Score,
                AttemptDate = r.AttemptDate,
                AssessmentTitle = r.Assessment?.Title,
                UserName = r.User?.Name
            });

            return Ok(dtoList);
        }

        // GET: api/Results/Assessment/{assessmentId}
        [HttpGet("Assessment/{assessmentId}")]
        [Authorize(Roles = "Instructor")]
        public async Task<ActionResult<IEnumerable<ResultReadDTO>>> GetResultsByAssessment(Guid assessmentId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            var assessment = await _context.Assessments
                .Include(a => a.Course)
                .FirstOrDefaultAsync(a => a.AssessmentId == assessmentId);

            if (assessment == null) return NotFound("Assessment not found");
            if (assessment.Course.InstructorId.ToString() != userId) return Forbid();

            var results = await _context.Results
                .Include(r => r.User)
                .Where(r => r.AssessmentId == assessmentId)
                .ToListAsync();

            var dtoList = results.Select(r => new ResultReadDTO
            {
                ResultId = r.ResultId,
                AssessmentId = r.AssessmentId,
                UserId = r.UserId,
                Score = r.Score,
                AttemptDate = r.AttemptDate,
                AssessmentTitle = assessment.Title,
                UserName = r.User?.Name
            });

            return Ok(dtoList);
        }
    }
}
