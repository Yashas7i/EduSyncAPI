using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EduSyncAPI.Models
{
    public class Assessment
    {
        [Key]
        public Guid AssessmentId { get; set; }

        [Required]
        public Guid CourseId { get; set; }

        [Required]
        public string? Title { get; set; }

        [Required]
        public string? Questions { get; set; } 

        [Required]
        public int MaxScore { get; set; }

        
        public Course? Course { get; set; }
        public ICollection<Result>? Results { get; set; }
    }
}
