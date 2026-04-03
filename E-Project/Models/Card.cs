using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_Project.Models
{
    public class Card
    {
        public int CardID { get; set; }

        [Required]
        public int CategoryID { get; set; }

        [ForeignKey("CategoryID")]
        public Category Category { get; set; }

        [MaxLength(200)]
        public string Title { get; set; }

        public string Message { get; set; }

        [MaxLength(500)]
        public string MediaPath { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
