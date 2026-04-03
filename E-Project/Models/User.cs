using System.ComponentModel.DataAnnotations;
using E_Project.Models;

public class User
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;   // DB column = Name

    [Required]
    public string Phone { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;

    public string Role { get; set; } = "User"; // Default role

    // Navigation properties
    public virtual ICollection<Subscription>? Subscriptions { get; set; }
    public virtual ICollection<CardSend>? CardSends { get; set; }
    public virtual ICollection<Feedback>? Feedbacks { get; set; }

    // Helper methods
    public Subscription? GetActiveSubscription()
    {
        return Subscriptions?.FirstOrDefault(s => s.IsActive());
    }

    public bool HasActiveSubscription()
    {
        return GetActiveSubscription() is not null;
    }

    public int GetEmailsSentToday()
    {
        var startOfDay = DateTime.Today;
        var endOfDay = startOfDay.AddDays(1);
        return CardSends?.Count(cs => cs.SentAt >= startOfDay && cs.SentAt < endOfDay) ?? 0;
    }

    public bool CanSendCard()
    {
        if (!HasActiveSubscription()) return false;
        return GetEmailsSentToday() < 10; // Pro plan limit: 10 emails per day
    }
}
