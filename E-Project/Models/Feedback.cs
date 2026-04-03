using System.ComponentModel.DataAnnotations;

namespace E_Project.Models
{
    public class Feedback
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string UserName { get; set; } = string.Empty;

        [Required, EmailAddress, MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required, MaxLength(1000)]
        public string Message { get; set; } = string.Empty;

        public DateTime SubmittedAt { get; set; } = DateTime.Now;

        public int? UserId { get; set; }  // Added to match database schema
        
        // Optional: Add rating field (1-5 stars)
        [Range(1, 5)]
        public int Rating { get; set; } = 5;  // Default to 5 stars
    }
}