using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EduSyncAPI.DTOs
{
    public class CourseCreateDTO
    {
        [Required]
        [MinLength(1)]
        public string? Title { get; set; }

        public string? Description { get; set; }
        public string? MediaUrl { get; set; }
    }

    public class CourseReadDTO
    {
        public Guid CourseId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? MediaUrl { get; set; }
        public Guid InstructorId { get; set; }
        public string? InstructorName { get; set; }

        public List<AssessmentSummaryDTO>? Assessments { get; set; }
    }

    public class CourseUpdateDTO
    {
        [Required]
        [MinLength(1)]
        public string? Title { get; set; }

        public string? Description { get; set; }
        public string? MediaUrl { get; set; }
    }
    public class CourseDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        // Add more fields as needed
    }

}
