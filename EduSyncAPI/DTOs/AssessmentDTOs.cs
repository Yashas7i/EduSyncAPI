namespace EduSyncAPI.DTOs
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class AssessmentCreateDTO
    {
        [Required]
        public Guid CourseId { get; set; }

        [Required]
        [StringLength(200)]
        public string? Title { get; set; }

        [Required]
        [MinLength(1)]
        public string? Questions { get; set; }

        [Required]
        public int MaxScore { get; set; }
    }

    public class AssessmentReadDTO
    {
        public Guid AssessmentId { get; set; }
        public Guid CourseId { get; set; }
        public string? Title { get; set; }
        public string? Questions { get; set; }
        public int MaxScore { get; set; }
        public string? CourseName { get; set; }
    }

    public class AssessmentUpdateDTO
    {
        [Required]
        [StringLength(200)]
        public string? Title { get; set; }

        [Required]
        [MinLength(1)]
        public string? Questions { get; set; }

        [Required]
        public int MaxScore { get; set; }
    }

    public class AssessmentSummaryDTO
    {
        public Guid AssessmentId { get; set; }
        public string? Title { get; set; }
    }
}
