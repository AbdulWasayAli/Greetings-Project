using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace E_Project.Models
{
    public class Category
    {
        public int CategoryID { get; set; }

        [Required, MaxLength(100)]
        public required string CategoryName { get; set; }

        [MaxLength(200)]
        public string? ImagePath { get; set; } // Category card ka image

        // Navigation
        public ICollection<Card> Cards { get; set; } = new List<Card>();
    }
}
