using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace EduSyncAPI.Models
{
    public class User
    {
        [Key]
        public Guid UserId { get; set; }

        [Required]
        public string? Name { get; set; }

        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        public string? Role { get; set; } 

        [Required]
        public string? PasswordHash { get; set; }

        
        public ICollection<Result>? Results { get; set; }
        public ICollection<Course>? CoursesTaught { get; set; }
    }
}
