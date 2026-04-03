using System.Collections.Generic;

namespace E_Project.Models
{
    public class SendCardModel
    {
        public string? Image { get; set; }
        public string? CustomImageData { get; set; } // For base64 custom images
        public string? Title { get; set; }
        public string? Message { get; set; }
        public List<string>? Emails { get; set; }
    }
}
