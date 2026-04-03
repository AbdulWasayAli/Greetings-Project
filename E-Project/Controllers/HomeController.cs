using E_Project.Data;
using E_Project.Models;
using E_Project.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;
namespace E_Project.Controllers
{
    [PreventAdminAccessToMainSiteFilter]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;


        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, IWebHostEnvironment hostingEnvironment)
        {
            _logger = logger;
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }

        // Index
        public async Task<IActionResult> Index(string search)
        {
            if (!string.IsNullOrEmpty(search))
            {
                var searchTerm = search.Trim().ToLower();
                
                // First try exact match (case-insensitive)
                var exactCategory = _context.Categories
                    .FirstOrDefault(c => c.CategoryName.ToLower() == searchTerm);

                if (exactCategory != null)
                {
                    return RedirectToAction("Cards", new { id = exactCategory.CategoryID });
                }

                // Then try partial match (contains)
                var partialCategory = _context.Categories
                    .FirstOrDefault(c => c.CategoryName.ToLower().Contains(searchTerm));

                if (partialCategory != null)
                {
                    return RedirectToAction("Cards", new { id = partialCategory.CategoryID });
                }

                // If no match found, suggest similar categories
                var allCategories = _context.Categories.ToList();
                var suggestions = GetSimilarCategories(searchTerm, allCategories);
                
                if (suggestions.Any())
                {
                    ViewBag.SearchTerm = search;
                    ViewBag.SearchSuggestions = suggestions;
                    ViewBag.SearchError = $"❌ No exact match found for '{search}'. Here are some suggestions:";
                }
                else
                {
                    ViewBag.SearchTerm = search;
                    ViewBag.SearchError = $"❌ Sorry, '{search}' doesn't match any category. Please check your spelling and try one of these popular categories:";
                    // Provide popular categories as fallback suggestions
                    var popularCategories = allCategories.Take(4).ToList();
                    ViewBag.SearchSuggestions = popularCategories;
                }
            }

            // Get categories for main display
            var categories = _context.Categories.ToList();
            
            // Get recent feedbacks for reviews section (latest 6)
            var recentFeedbacks = await _context.Feedbacks
                .OrderByDescending(f => f.SubmittedAt)
                .Take(6)
                .Select(f => new {
                    f.Id,
                    f.UserName,
                    f.Message,
                    f.SubmittedAt,
                    f.Rating,
                    // Create a shortened message for display
                    ShortMessage = f.Message.Length > 80 ? f.Message.Substring(0, 80) + "..." : f.Message
                })
                .ToListAsync();

            // Process time formatting after database query
            var processedFeedbacks = recentFeedbacks.Select(f => new {
                f.Id,
                f.UserName,
                f.Message,
                f.SubmittedAt,
                f.Rating,
                f.ShortMessage,
                TimeAgo = GetTimeAgo(f.SubmittedAt)
            }).ToList();

            // Pass both categories and feedbacks to view
            ViewBag.RecentFeedbacks = processedFeedbacks;
            return View(categories);
        }

        // Helper method to create "X days ago" format - made static to avoid EF issues
        private static string GetTimeAgo(DateTime date)
        {
            var timeSpan = DateTime.Now - date;
            
            if (timeSpan.TotalDays >= 1)
            {
                int days = (int)timeSpan.TotalDays;
                return $"{days} day{(days == 1 ? "" : "s")} ago";
            }
            else if (timeSpan.TotalHours >= 1)
            {
                int hours = (int)timeSpan.TotalHours;
                return $"{hours} hour{(hours == 1 ? "" : "s")} ago";
            }
            else
            {
                int minutes = Math.Max(1, (int)timeSpan.TotalMinutes);
                return $"{minutes} minute{(minutes == 1 ? "" : "s")} ago";
            }
        }




        // Cards
        public async Task<IActionResult> Cards(int id)
        {
            var cards = await _context.Cards
                                      .Where(c => c.CategoryID == id)
                                      .ToListAsync();

            var categoryName = await _context.Categories
                                             .Where(c => c.CategoryID == id)
                                             .Select(c => c.CategoryName)
                                             .FirstOrDefaultAsync();

            ViewBag.CategoryName = categoryName;
            ViewData["Title"] = categoryName;  // ?? ye layout me else block lega

            // Check user subscription status
            var userName = HttpContext.Session.GetString("UserName");
            ViewBag.IsLoggedIn = !string.IsNullOrEmpty(userName);
            ViewBag.HasActiveSubscription = false;

            if (!string.IsNullOrEmpty(userName))
            {
                var user = await _context.Users
                    .Include(u => u.Subscriptions)
                    .FirstOrDefaultAsync(u => u.Name == userName);
                
                ViewBag.HasActiveSubscription = user?.HasActiveSubscription() ?? false;
            }

            return View(cards);
        }



        public IActionResult About()
        {
            ViewBag.Title = "About";
            return View();
        }
        public IActionResult Contact()
        {
            ViewBag.Title = "Contact";
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Contact(ContactMessage model)
        {
            if (ModelState.IsValid)
            {
                model.CreatedAt = DateTime.Now;
                _context.ContactMessages.Add(model);
                _context.SaveChanges();

                // Set TempData for success message
                TempData["SuccessMessage"] = "Your message has been sent successfully!";

                // Redirect back to same Contact page
                return RedirectToAction("Contact");
            }

            return View(model);
        }


        public IActionResult Feedback()
        {
            ViewBag.Title = "Feedback";
            return View();
        }
        public IActionResult Login()
        {
            ViewBag.Title = "Login";
            ViewBag.HideSubHeader = true;
            return View();
        }
        // POST: Login
        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                TempData["ErrorMessage"] = "Email and Password are required!";
                ViewBag.HideSubHeader = true;  // 👈 Add this
                return View();
            }

            var user = _context.Users.FirstOrDefault(x => x.Email == email && x.Password == password);

            if (user != null)
            {
                HttpContext.Session.SetString("UserName", user.Name);
                HttpContext.Session.SetString("UserRole", user.Role);

                TempData["LoginSuccess"] = $"Login Successful! Welcome {user.Name}";

                if (user.Role == "Admin")
                {
                    return RedirectToAction("Dashboard", "Admin");
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }

            TempData["ErrorMessage"] = "Invalid Email or Password!";
            ViewBag.HideSubHeader = true;   // 👈 Add this
            return View();
        }


        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData.Clear();

            // ✅ Alag key rakho logout ke liye
            TempData["LogoutMessage"] = "You have been logged out successfully!";
            return RedirectToAction("Login");
        }



        public IActionResult Register()
        {
            ViewBag.Title = "Register";
            ViewBag.HideSubHeader = true; 
            return View();
        }

        // POST: Register
        [HttpPost]
        public IActionResult Register(User user)
        {
            if (ModelState.IsValid)
            {
                // ? By default role hamesha User rahega
                user.Role = "User";
                _context.Users.Add(user);
                _context.SaveChanges();

                //TempData["SuccessMessage"] = "Registered Successfully!";
                return RedirectToAction("Login");
            }
            return View(user);
        }

        // GET: SendCard - Display card sending page
        public IActionResult SendCard()
        {
            ViewBag.Title = "Send Card";
            
            // Check if user is logged in
            var userName = HttpContext.Session.GetString("UserName");
            if (string.IsNullOrEmpty(userName))
            {
                TempData["ErrorMessage"] = "Please login to send cards";
                return RedirectToAction("Login");
            }

            // Get user's current subscription status
            var user = _context.Users
                .Include(u => u.Subscriptions)
                .Include(u => u.CardSends)
                .FirstOrDefault(u => u.Name == userName);

            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found";
                return RedirectToAction("Login");
            }

            // Check if user has active subscription
            if (!user.HasActiveSubscription())
            {
                TempData["ErrorMessage"] = "Pro subscription required to send cards. Please subscribe to continue!";
                return RedirectToAction("Subscribe");
            }

            ViewBag.User = user;
            ViewBag.HasActiveSubscription = true;
            
            // Calculate emails sent today
            var emailsSentToday = user.GetEmailsSentToday();
            var cardsRemaining = 10 - emailsSentToday;
            
            // Add some debug info to help troubleshoot
            _logger.LogInformation($"User '{userName}' - Emails sent today: {emailsSentToday}, Cards remaining: {cardsRemaining}");
            
            ViewBag.CardsRemaining = cardsRemaining;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SendCard([FromBody] SendCardModel model)
        {
            _logger.LogInformation("SendCard action called");
            
            if (model == null)
            {
                _logger.LogWarning("SendCard: Model is null");
                return Json(new { success = false, error = "No data provided" });
            }

            _logger.LogInformation("SendCard: Received model - Image: {Image}, Title: {Title}, EmailCount: {EmailCount}, HasCustomImageData: {HasCustomImageData}", 
                model.Image, model.Title, model.Emails?.Count ?? 0, !string.IsNullOrEmpty(model.CustomImageData));

            if (model.Emails == null || model.Emails.Count == 0)
            {
                _logger.LogWarning("SendCard: No emails provided");
                return Json(new { success = false, error = "No emails provided" });
            }

            // Check if user is logged in
            var userName = HttpContext.Session.GetString("UserName");
            _logger.LogInformation("SendCard: UserName from session: {UserName}", userName ?? "null");
            
            if (string.IsNullOrEmpty(userName))
            {
                _logger.LogWarning("SendCard: User not logged in");
                return Json(new { success = false, error = "Please login to send cards" });
            }

            // Get current user
            var user = await _context.Users
                .Include(u => u.Subscriptions)
                .Include(u => u.CardSends)
                .FirstOrDefaultAsync(u => u.Name == userName);

            if (user == null)
            {
                _logger.LogWarning("SendCard: User not found for UserName: {UserName}", userName);
                return Json(new { success = false, error = "User not found" });
            }

            _logger.LogInformation("SendCard: Found user {UserId} with {SubscriptionCount} subscriptions", 
                user.Id, user.Subscriptions?.Count ?? 0);

            // Check if user has active subscription
            var hasActiveSubscription = user.HasActiveSubscription();
            _logger.LogInformation("SendCard: User has active subscription: {HasActiveSubscription}", hasActiveSubscription);
            
            if (!hasActiveSubscription)
            {
                _logger.LogWarning("SendCard: User {UserId} does not have active subscription", user.Id);
                return Json(new { 
                    success = false, 
                    error = "Pro subscription required to send cards. Please subscribe to continue!",
                    requiresSubscription = true
                });
            }

            // Check daily sending limit (10 emails for Pro plan)
            var canSendCard = user.CanSendCard();
            _logger.LogInformation("SendCard: User can send card: {CanSendCard}", canSendCard);
            
            if (!canSendCard)
            {
                var emailsSent = user.GetEmailsSentToday();
                _logger.LogWarning("SendCard: User {UserId} has reached daily limit. Sent: {EmailsSent}", user.Id, emailsSent);
                return Json(new { 
                    success = false, 
                    error = $"Daily limit reached! You have sent {emailsSent}/10 emails today. Try again tomorrow.",
                    limitReached = true
                });
            }

            try
            {
                _logger.LogInformation("SendCard: Starting email sending process for {EmailCount} emails", model.Emails.Count);
                
                // Find the card being sent (optional - null for templates and custom images)
                Card? card = null;
                if (!string.IsNullOrEmpty(model.Image) && model.Image != "custom")
                {
                    // Try to find an existing card that matches the template
                    card = await _context.Cards.FirstOrDefaultAsync(c => 
                        c.MediaPath != null && c.MediaPath.Contains(model.Image));
                    _logger.LogInformation("SendCard: Found card for template {Template}: {CardId}", model.Image, card?.CardID);
                }
                
                var sentCards = new List<CardSend>();

                foreach (var email in model.Emails)
                {
                    // Create record of sent card
                    var cardSend = new CardSend
                    {
                        UserId = user.Id,
                        CardId = card?.CardID, // Nullable - will be null for templates and custom images
                        RecipientEmail = email,
                        RecipientName = email.Split('@')[0], // Use part before @ as name
                        Message = model.Message,
                        SentAt = DateTime.Now,
                        Status = "Sent"
                    };

                    _context.CardSends.Add(cardSend);
                    sentCards.Add(cardSend);

                    _logger.LogInformation("SendCard: Processing email for {Email}", email);
                    
                    // Send email with styled template
                    string imageUrl = "";
                    string templateName = "";
                    string imagePath = "";
                    string baseUrl = $"{Request.Scheme}://{Request.Host}";
                    
                    _logger.LogInformation("SendCard: Processing image. Type: {ImageType}, CustomData: {HasCustomData}", 
                        model.Image, !string.IsNullOrEmpty(model.CustomImageData));
                    
                    if (model.Image == "custom" && !string.IsNullOrEmpty(model.CustomImageData))
                    {
                        // Handle custom image - save to file system and get both URL and path
                        var customImageResult = await SaveBase64ImageWithPath(model.CustomImageData, baseUrl);
                        imageUrl = $"cid:greeting-card-image"; // Use Content-ID reference for embedded image
                        templateName = "Custom Image";
                        imagePath = customImageResult.FilePath; // Use the file path for email attachment
                        _logger.LogInformation("SendCard: Custom image saved to {ImagePath}", imagePath);
                    }
                    else if (!string.IsNullOrEmpty(model.Image))
                    {
                        // Handle template image
                        
                        // Map template names to image files
                        var templateImages = new Dictionary<string, string>
                        {
                            {"birthday", "1c.jpg"},
                            {"wedding", "2c.jpg"},
                            {"newyear", "3c.jpg"},
                            {"eid-fitr", "4c.jpg"},
                            {"eid-adha", "5c.jpg"},
                            {"christmas", "6c.jpg"},
                            {"thanksgiving", "7c.webp"},
                            {"diwali", "8c.jpg"}
                        };
                        
                        if (templateImages.ContainsKey(model.Image))
                        {
                            var imageFileName = templateImages[model.Image];
                            imagePath = Path.Combine(_hostingEnvironment.WebRootPath, "User", "images", imageFileName);
                            imageUrl = $"cid:greeting-card-image"; // Use Content-ID reference for embedded image
                            templateName = char.ToUpper(model.Image[0]) + model.Image.Substring(1);
                            _logger.LogInformation("SendCard: Template {Template} mapped to {ImagePath}", model.Image, imagePath);
                        }
                        else
                        {
                            // Fallback for unknown templates
                            imagePath = Path.Combine(_hostingEnvironment.WebRootPath, "User", "images", "1c.jpg");
                            imageUrl = $"cid:greeting-card-image";
                            templateName = "Default Card";
                            _logger.LogWarning("SendCard: Unknown template {Template}, using default", model.Image);
                        }
                    }
                    else
                    {
                        // If no image specified, use default
                        imagePath = Path.Combine(_hostingEnvironment.WebRootPath, "User", "images", "1c.jpg");
                        imageUrl = $"cid:greeting-card-image";
                        templateName = "Default Card";
                        _logger.LogInformation("SendCard: No image specified, using default");
                    }
                    
                    // Verify image file exists
                    if (!string.IsNullOrEmpty(imagePath) && !System.IO.File.Exists(imagePath))
                    {
                        _logger.LogWarning("SendCard: Image file does not exist: {ImagePath}", imagePath);
                    }

                    var message = new MailMessage();
                    message.From = new MailAddress("ammijackson13@gmail.com", "E-Card Service");
                    message.To.Add(email);
                    message.Subject = model.Title;
                    message.Body = GenerateStyledEmailHtml(model.Title ?? "", model.Message ?? "", imageUrl, templateName);
                    message.IsBodyHtml = true;

                    // Embed image as attachment if it's a local template image
                    if (!string.IsNullOrEmpty(imagePath) && System.IO.File.Exists(imagePath))
                    {
                        var attachment = new Attachment(imagePath);
                        attachment.ContentId = "greeting-card-image";
                        if (attachment.ContentDisposition != null)
                        {
                            attachment.ContentDisposition.Inline = true;
                            attachment.ContentDisposition.DispositionType = "inline";
                        }
                        message.Attachments.Add(attachment);
                    }

                    try
                    {
                        // Try to resolve DNS first for diagnostics
                        try
                        {
                            var hostEntry = System.Net.Dns.GetHostEntry("smtp.gmail.com");
                            _logger.LogInformation("DNS resolution successful for smtp.gmail.com: {Addresses}", 
                                string.Join(", ", hostEntry.AddressList.Select(a => a.ToString())));
                        }
                        catch (Exception dnsEx)
                        {
                            _logger.LogError(dnsEx, "DNS resolution failed for smtp.gmail.com - trying IP address");
                        }

                        // Try multiple approaches
                        bool emailSent = false;
                        Exception lastException = null;

                        // Approach 1: Standard Gmail SMTP
                        if (!emailSent)
                        {
                            try
                            {
                                using (var smtp = new SmtpClient())
                                {
                                    smtp.Host = "smtp.gmail.com";
                                    smtp.Port = 587;
                                    smtp.Credentials = new NetworkCredential("ammijackson13@gmail.com", "pmnx hqdu bedc nwob");
                                    smtp.EnableSsl = true;
                                    smtp.Timeout = 60000;
                                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                                    
                                    _logger.LogInformation("Trying standard Gmail SMTP: {Host}:{Port}", smtp.Host, smtp.Port);
                                    smtp.Send(message);
                                    emailSent = true;
                                    _logger.LogInformation("Email sent successfully via standard SMTP to {Email}", email);
                                }
                            }
                            catch (Exception ex)
                            {
                                lastException = ex;
                                _logger.LogWarning(ex, "Standard Gmail SMTP failed");
                            }
                        }

                        // Approach 2: Try Gmail SMTP on port 465 (SSL)
                        if (!emailSent)
                        {
                            try
                            {
                                using (var smtp = new SmtpClient())
                                {
                                    smtp.Host = "smtp.gmail.com";
                                    smtp.Port = 465;
                                    smtp.Credentials = new NetworkCredential("ammijackson13@gmail.com", "pmnx hqdu bedc nwob");
                                    smtp.EnableSsl = true;
                                    smtp.Timeout = 60000;
                                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                                    
                                    _logger.LogInformation("Trying Gmail SMTP SSL: {Host}:{Port}", smtp.Host, smtp.Port);
                                    smtp.Send(message);
                                    emailSent = true;
                                    _logger.LogInformation("Email sent successfully via SSL SMTP to {Email}", email);
                                }
                            }
                            catch (Exception ex)
                            {
                                lastException = ex;
                                _logger.LogWarning(ex, "Gmail SMTP SSL failed");
                            }
                        }

                        // Approach 3: Try using IP address directly
                        if (!emailSent)
                        {
                            try
                            {
                                using (var smtp = new SmtpClient())
                                {
                                    smtp.Host = "142.251.168.108"; // Gmail SMTP IP
                                    smtp.Port = 587;
                                    smtp.Credentials = new NetworkCredential("ammijackson13@gmail.com", "pmnx hqdu bedc nwob");
                                    smtp.EnableSsl = true;
                                    smtp.Timeout = 60000;
                                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                                    
                                    _logger.LogInformation("Trying Gmail SMTP via IP: {Host}:{Port}", smtp.Host, smtp.Port);
                                    smtp.Send(message);
                                    emailSent = true;
                                    _logger.LogInformation("Email sent successfully via IP SMTP to {Email}", email);
                                }
                            }
                            catch (Exception ex)
                            {
                                lastException = ex;
                                _logger.LogWarning(ex, "Gmail SMTP via IP failed");
                            }
                        }

                        if (!emailSent && lastException != null)
                        {
                            throw lastException;
                        }
                    }
                    catch (Exception emailEx)
                    {
                        _logger.LogError(emailEx, "Failed to send email to {Email}", email);
                        throw new Exception($"Failed to send email to {email}: {emailEx.Message}", emailEx);
                    }
                }

                // Save all card send records to database
                await _context.SaveChangesAsync();

                // Get updated stats (refresh user from database to get accurate count)
                if (user.CardSends != null)
                {
                    await _context.Entry(user).Collection(u => u.CardSends!).LoadAsync();
                }
                var emailsSentToday = user.GetEmailsSentToday();
                var remainingEmails = 10 - emailsSentToday;

                return Json(new { 
                    success = true, 
                    message = $"Emails sent successfully! You have {remainingEmails} emails remaining today.",
                    cardsSent = sentCards.Count,
                    remainingCards = remainingEmails,
                    emailsSentToday = emailsSentToday
                });
            }
            catch (Exception ex)
            {
                // Log the full exception for debugging
                _logger.LogError(ex, "Failed to send cards for user {UserName}", userName);
                
                // Return detailed error information
                string errorMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    errorMessage += " Inner: " + ex.InnerException.Message;
                }
                
                return Json(new { 
                    success = false, 
                    error = "Failed to send cards: " + errorMessage,
                    details = ex.ToString() // Include full stack trace for debugging
                });
            }
        }

        // Helper method to generate styled email HTML that matches the preview card
        private string GenerateStyledEmailHtml(string title, string message, string imageUrl, string templateName = "")
        {
            return $@"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>{title}</title>
    <style>
        * {{
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }}
        body {{
            margin: 0;
            padding: 20px;
            font-family: Arial, Helvetica, sans-serif;
            background-color: #f8f9fa;
            -webkit-text-size-adjust: 100%;
        }}
        .email-container {{
            max-width: 600px;
            width: 100%;
            margin: 0 auto;
            background-color: white;
            border-radius: 15px;
            overflow: hidden;
            box-shadow: 0 10px 30px rgba(0,0,0,0.1);
        }}
        .card-header {{
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            text-align: center;
            padding: 20px;
        }}
        .card-header h1 {{
            margin: 0;
            font-size: 24px;
            font-weight: 600;
            color: white;
        }}
        .card-image {{
            width: 100%;
            max-width: 100%;
            height: 300px;
            object-fit: cover;
            display: block;
            border: none;
        }}
        .card-body {{
            padding: 30px;
            text-align: center;
        }}
        .card-title {{
            color: #007bff;
            font-size: 22px;
            font-weight: 600;
            margin-bottom: 15px;
            line-height: 1.3;
        }}
        .card-message {{
            color: #495057;
            font-size: 16px;
            line-height: 1.6;
            margin-bottom: 20px;
        }}
        .card-footer {{
            background-color: #f8f9fa;
            padding: 20px;
            text-align: center;
            border-top: 1px solid #dee2e6;
        }}
        .template-info {{
            color: #6c757d;
            font-size: 12px;
            margin-bottom: 10px;
        }}
        .branding {{
            color: #6c757d;
            font-size: 14px;
            margin: 0;
        }}
        .branding a {{
            color: #007bff;
            text-decoration: none;
        }}
        .decorative-border {{
            height: 4px;
            background: linear-gradient(90deg, #ff6b6b, #4ecdc4, #45b7d1, #96ceb4, #feca57);
        }}
        
        /* Email client compatibility */
        table {{
            border-collapse: collapse;
            border-spacing: 0;
        }}
        
        @media only screen and (max-width: 600px) {{
            .email-container {{
                margin: 10px;
                border-radius: 10px;
            }}
            .card-body {{
                padding: 20px;
            }}
            .card-title {{
                font-size: 20px;
            }}
            .card-message {{
                font-size: 15px;
            }}
        }}
    </style>
</head>
<body>
    <div class='email-container'>
        <div class='decorative-border'></div>
        <div class='card-header'>
            <h1>🎉 Special Greeting Card</h1>
        </div>
        <div style='text-align: center; line-height: 0;'>
            <img src='{imageUrl}' alt='Greeting Card' class='card-image' width='600' height='300' style='width: 100%; max-width: 600px; height: 300px; object-fit: cover; display: block; border: none;' />
        </div>
        <div class='card-body'>
            <h2 class='card-title'>{title}</h2>
            <p class='card-message'>{message}</p>
        </div>
        <div class='card-footer'>
            {(string.IsNullOrEmpty(templateName) ? "" : $"<p class='template-info'>Template: {templateName}</p>")}
            <p class='branding'>Sent with love using E-Project Greeting Cards 💝</p>
        </div>
        <div class='decorative-border'></div>
    </div>
</body>
</html>";
        }

        // Helper method to save base64 image to file system
        private async Task<string> SaveBase64Image(string base64Data, string baseUrl)
        {
            try
            {
                // Remove the data:image/...;base64, prefix if present
                if (base64Data.Contains(','))
                {
                    base64Data = base64Data.Split(',')[1];
                }

                byte[] imageBytes = Convert.FromBase64String(base64Data);
                
                // Generate unique filename
                string fileName = $"custom_{Guid.NewGuid().ToString("N")[..8]}.png";
                string uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                
                // Create uploads directory if it doesn't exist
                if (!Directory.Exists(uploadsPath))
                {
                    Directory.CreateDirectory(uploadsPath);
                }
                
                string filePath = Path.Combine(uploadsPath, fileName);
                await System.IO.File.WriteAllBytesAsync(filePath, imageBytes);
                
                // Return the URL
                return $"{baseUrl}/uploads/{fileName}";
            }
            catch (Exception)
            {
                // If saving fails, return the base64 data URL as fallback
                return $"data:image/png;base64,{base64Data}";
            }
        }

        // Helper method to save base64 image and return both URL and file path
        private async Task<(string Url, string FilePath)> SaveBase64ImageWithPath(string base64Data, string baseUrl)
        {
            try
            {
                // Remove the data:image/...;base64, prefix if present
                if (base64Data.Contains(','))
                {
                    base64Data = base64Data.Split(',')[1];
                }

                byte[] imageBytes = Convert.FromBase64String(base64Data);
                
                // Generate unique filename
                string fileName = $"custom_{Guid.NewGuid().ToString("N")[..8]}.png";
                string uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                
                // Create uploads directory if it doesn't exist
                if (!Directory.Exists(uploadsPath))
                {
                    Directory.CreateDirectory(uploadsPath);
                }
                
                string filePath = Path.Combine(uploadsPath, fileName);
                await System.IO.File.WriteAllBytesAsync(filePath, imageBytes);
                
                // Return both URL and file path
                return ($"{baseUrl}/uploads/{fileName}", filePath);
            }
            catch (Exception)
            {
                // If saving fails, return fallback values
                return ($"data:image/png;base64,{base64Data}", "");
            }
        }

        // Helper method to find similar categories based on Levenshtein distance and common patterns
        private List<Category> GetSimilarCategories(string searchTerm, List<Category> allCategories)
        {
            var suggestions = new List<Category>();

            // Common misspellings and alternatives
            var commonMisspellings = new Dictionary<string, string[]>
            {
                {"birthday", new[] {"birthday", "birthda", "bday", "birth", "borthday", "birtday", "birhday", "berth", "birthdy"}},
                {"wedding", new[] {"wedding", "weading", "weding", "marriage", "marry", "wedng", "wedin", "weddin", "weding"}},
                {"christmas", new[] {"christmas", "xmas", "christma", "chistmas", "chrismas", "cristmas", "chrimas", "christms", "x-mas"}},
                {"diwali", new[] {"diwali", "deepavali", "divali", "diwalli", "dipavali", "diwal", "dewali", "deepawali"}},
                {"thanksgiving", new[] {"thanksgiving", "thanks", "turkey", "thanksgiv", "thanksgivin", "thanks-giving"}},
                {"new year", new[] {"newyear", "new year", "ny", "year", "new-year", "newyears", "new years"}},
                {"eid", new[] {"eid", "id", "eed", "ied", "aid"}},
                {"valentine", new[] {"valentine", "valentines", "valentine's", "valentin", "vday", "v-day"}},
                {"anniversary", new[] {"anniversary", "aniversary", "anni", "anniversry", "aniversery"}}
            };

            // First, check for exact matches in misspellings
            foreach (var category in allCategories)
            {
                var categoryName = category.CategoryName.ToLower();
                
                if (commonMisspellings.ContainsKey(categoryName))
                {
                    var patterns = commonMisspellings[categoryName];
                    if (patterns.Any(pattern => 
                        LevenshteinDistance(searchTerm, pattern) <= 2 || 
                        pattern.Contains(searchTerm) || 
                        searchTerm.Contains(pattern)))
                    {
                        suggestions.Add(category);
                        continue;
                    }
                }

                // Also check direct Levenshtein distance (allow up to 3 character differences for longer words)
                var maxDistance = searchTerm.Length <= 4 ? 1 : (searchTerm.Length <= 8 ? 2 : 3);
                if (LevenshteinDistance(searchTerm, categoryName) <= maxDistance)
                {
                    suggestions.Add(category);
                }
                
                // Check for partial matches too (but only for longer search terms)
                if (searchTerm.Length >= 4 && (categoryName.Contains(searchTerm) || searchTerm.Contains(categoryName)))
                {
                    suggestions.Add(category);
                }
            }

            return suggestions.Distinct().Take(4).OrderBy(c => LevenshteinDistance(searchTerm, c.CategoryName.ToLower())).ToList();
        }

        // Calculate Levenshtein distance between two strings
        private int LevenshteinDistance(string source, string target)
        {
            if (string.IsNullOrEmpty(source)) return target?.Length ?? 0;
            if (string.IsNullOrEmpty(target)) return source.Length;

            var matrix = new int[source.Length + 1, target.Length + 1];

            for (int i = 0; i <= source.Length; i++)
                matrix[i, 0] = i;
            for (int j = 0; j <= target.Length; j++)
                matrix[0, j] = j;

            for (int i = 1; i <= source.Length; i++)
            {
                for (int j = 1; j <= target.Length; j++)
                {
                    int cost = (target[j - 1] == source[i - 1]) ? 0 : 1;
                    matrix[i, j] = Math.Min(
                        Math.Min(matrix[i - 1, j] + 1, matrix[i, j - 1] + 1),
                        matrix[i - 1, j - 1] + cost);
                }
            }

            return matrix[source.Length, target.Length];
        }






        public IActionResult Subscribe()
        {
            ViewBag.Title = "Subscribe";
            
            // Check if user is logged in
            var userName = HttpContext.Session.GetString("UserName");
            if (string.IsNullOrEmpty(userName))
            {
                TempData["ErrorMessage"] = "Please login to subscribe";
                return RedirectToAction("Login");
            }

            // Get user's current subscription status
            var user = _context.Users
                .Include(u => u.Subscriptions)
                .FirstOrDefault(u => u.Name == userName);

            ViewBag.User = user;
            ViewBag.HasActiveSubscription = user?.HasActiveSubscription() ?? false;
            ViewBag.CurrentSubscription = user?.GetActiveSubscription();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> PurchaseSubscription([FromBody] PurchaseSubscriptionRequest request)
        {
            // Check if user is logged in
            var userName = HttpContext.Session.GetString("UserName");
            if (string.IsNullOrEmpty(userName))
            {
                return Json(new { success = false, message = "Please login to purchase subscription" });
            }

            try
            {
                // Get current user
                var user = await _context.Users
                    .Include(u => u.Subscriptions)
                    .FirstOrDefaultAsync(u => u.Name == userName);

                if (user == null)
                {
                    return Json(new { success = false, message = "User not found" });
                }

                // Check if user already has an active subscription
                var existingSubscription = user.GetActiveSubscription();
                if (existingSubscription != null)
                {
                    return Json(new { 
                        success = false, 
                        message = $"You already have an active Pro subscription until {existingSubscription.EndDate:yyyy-MM-dd}" 
                    });
                }

                // Create new subscription (simulate payment processing)
                var subscription = new Subscription
                {
                    UserId = user.Id,
                    PlanType = "Pro",
                    Amount = 9.99m,
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddMonths(1),
                    Status = "Active",
                    PaymentMethod = request.PaymentMethod,
                    TransactionId = Guid.NewGuid().ToString()
                };

                _context.Subscriptions.Add(subscription);
                await _context.SaveChangesAsync();

                return Json(new { 
                    success = true, 
                    message = "Pro subscription activated successfully! You can now send up to 10 cards per month.",
                    subscription = new {
                        planType = subscription.PlanType,
                        startDate = subscription.StartDate.ToString("yyyy-MM-dd"),
                        endDate = subscription.EndDate.ToString("yyyy-MM-dd"),
                        amount = subscription.Amount,
                        transactionId = subscription.TransactionId
                    }
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error processing subscription: " + ex.Message });
            }
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // POST: Feedback - Submit user feedback
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitFeedback([FromForm] string userName, [FromForm] string email, 
            [FromForm] string message, [FromForm] int rating = 5)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(message))
            {
                return Json(new { success = false, error = "Please fill all fields correctly." });
            }

            // Validate rating (should be between 1 and 5)
            if (rating < 1 || rating > 5)
            {
                rating = 5; // Default to 5 if invalid
            }

            // Try to get the current user's ID from session (optional)
            var currentUserName = HttpContext.Session.GetString("UserName");
            int? currentUserId = null;
            if (!string.IsNullOrEmpty(currentUserName))
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Name == currentUserName);
                currentUserId = user?.Id;
            }

            var feedback = new Feedback
            {
                UserName = userName,
                Email = email,
                Message = message,
                Rating = rating,
                SubmittedAt = DateTime.Now,
                UserId = currentUserId  // Will be null for anonymous users
            };

            _context.Feedbacks.Add(feedback);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Thank you for your feedback!" });
        }

        // GET: GetEmailCount - Returns current email count for logged in user
        [HttpGet]
        public async Task<IActionResult> GetEmailCount()
        {
            // Check if user is logged in
            var userName = HttpContext.Session.GetString("UserName");
            if (string.IsNullOrEmpty(userName))
            {
                return Json(new { success = false, error = "Please login to check email count" });
            }

            // Get current user
            var user = await _context.Users
                .Include(u => u.Subscriptions)
                .Include(u => u.CardSends)
                .FirstOrDefaultAsync(u => u.Name == userName);

            if (user == null)
            {
                return Json(new { success = false, error = "User not found" });
            }

            // Check if user has active subscription
            if (!user.HasActiveSubscription())
            {
                return Json(new { 
                    success = false, 
                    error = "Pro subscription required",
                    remainingCards = 0,
                    hasActiveSubscription = false
                });
            }

            // Get current email count
            var emailsSentToday = user.GetEmailsSentToday();
            var remainingEmails = 10 - emailsSentToday;

            return Json(new { 
                success = true, 
                remainingCards = remainingEmails,
                emailsSentToday = emailsSentToday,
                hasActiveSubscription = true
            });
        }
    }

    // Helper class for purchase subscription requests
    public class PurchaseSubscriptionRequest
    {
        public string PaymentMethod { get; set; } = "Card";
        public string CardNumber { get; set; } = "";
        public string CardHolder { get; set; } = "";
        public string ExpiryDate { get; set; } = "";
        public string CVV { get; set; } = "";
    }
}
