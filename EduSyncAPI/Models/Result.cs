using System;
using System.ComponentModel.DataAnnotations;

namespace EduSyncAPI.Models
{
    public class Result
    {
        [Key]
        public Guid ResultId { get; set; }

        [Required]
        public Guid AssessmentId { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required]
        public int Score { get; set; }

        public DateTime AttemptDate { get; set; }

        // Navigation
        public Assessment? Assessment { get; set; }
        public User? User { get; set; }
    }
}
