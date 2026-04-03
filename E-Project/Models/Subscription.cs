using System.ComponentModel.DataAnnotations;

namespace E_Project.Models
{
    public class Subscription
    {
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public string PlanType { get; set; } = "Pro"; // Pro, Enterprise, etc.

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public string Status { get; set; } = "Active"; // Active, Expired, Cancelled

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string PaymentMethod { get; set; } = "Card"; // Card, PayPal, etc.

        public string TransactionId { get; set; } = string.Empty;

        // Navigation property
        public virtual User User { get; set; } = null!;

        // Helper methods
        public bool IsActive()
        {
            return Status == "Active" && EndDate > DateTime.Now;
        }

        public int DaysRemaining()
        {
            if (!IsActive()) return 0;
            return (int)(EndDate - DateTime.Now).TotalDays;
        }
    }
}