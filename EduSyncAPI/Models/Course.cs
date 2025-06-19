using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EduSyncAPI.Models
{
    public class Course
    {
        [Key]
        public Guid CourseId { get; set; }

        [Required]
        public string? Title { get; set; }

        public string? Description { get; set; }

        public string? MediaUrl { get; set; }

        [Required]
        public Guid InstructorId { get; set; }

        
        public User? Instructor { get; set; }
        public ICollection<Assessment>? Assessments { get; set; }
    }
}
