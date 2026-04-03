using System.ComponentModel.DataAnnotations;

namespace E_Project.Models
{
    public class CardSend
    {
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        // Made nullable since we might send template/custom cards that don't exist in Cards table
        public int? CardId { get; set; }

        [Required, EmailAddress]
        public string RecipientEmail { get; set; } = string.Empty;

        [Required]
        public string RecipientName { get; set; } = string.Empty;

        public string? Message { get; set; }

        [Required]
        public DateTime SentAt { get; set; } = DateTime.Now;

        public string Status { get; set; } = "Sent"; // Sent, Delivered, Failed

        // Navigation properties
        public virtual User? User { get; set; }
        public virtual Card? Card { get; set; }
    }
}