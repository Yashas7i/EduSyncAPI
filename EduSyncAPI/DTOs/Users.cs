using System;
using System.ComponentModel.DataAnnotations;

namespace EduSyncAPI.DTOs
{
    public class UserReadDTO
    {
        public Guid UserId { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
    }

    public class UserUpdateDTO
    {
        [Required]
        [StringLength(100)]
        public string? Name { get; set; }

        [Required]
        [StringLength(100)]
        [EmailAddress]
        public string? Email { get; set; }

        public string? Password { get; set; }
    }
}
