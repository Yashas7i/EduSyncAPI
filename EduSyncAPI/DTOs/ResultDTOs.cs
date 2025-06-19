using System;
using System.ComponentModel.DataAnnotations;

namespace EduSyncAPI.DTOs
{
    public class ResultCreateDTO
    {
        [Required]
        public Guid AssessmentId { get; set; }

        [Required]
        public int Score { get; set; }
    }

    public class ResultReadDTO
    {
        public Guid ResultId { get; set; }
        public Guid AssessmentId { get; set; }
        public Guid UserId { get; set; }
        public int Score { get; set; }
        public DateTime AttemptDate { get; set; }
        public string? AssessmentTitle { get; set; }
        public string? UserName { get; set; }
    }
}
