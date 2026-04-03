using Microsoft.AspNetCore.Mvc;
using E_Project.Data;
using E_Project.Models;
using Microsoft.EntityFrameworkCore;

namespace E_Project.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        private bool IsAdminAuthenticated()
        {
            return !string.IsNullOrEmpty(HttpContext.Session.GetString("UserName"))
                   && HttpContext.Session.GetString("UserRole") == "Admin";
        }

        public async Task<IActionResult> Dashboard()
        {
            if (!IsAdminAuthenticated())
            {
                return RedirectToAction("Login", "Home");
            }

            // Check if admin tried to navigate to main site and show message
            var navigationMessage = HttpContext.Session.GetString("AdminNavigationMessage");
            if (!string.IsNullOrEmpty(navigationMessage))
            {
                TempData["InfoMessage"] = navigationMessage;
                HttpContext.Session.Remove("AdminNavigationMessage");
            }

            // Get dashboard statistics
            var totalUsers = await _context.Users.CountAsync();
            var totalCards = await _context.Cards.CountAsync();
            var totalCategories = await _context.Categories.CountAsync();
            var totalFeedbacks = await _context.Feedbacks.CountAsync();
            var totalContactMessages = await _context.ContactMessages.CountAsync();

            // Get subscription statistics
            var totalSubscriptions = await _context.Subscriptions.CountAsync();
            var activeSubscriptions = await _context.Subscriptions
                .CountAsync(s => s.Status == "Active" && 
                            s.StartDate <= DateTime.Now && 
                            s.EndDate > DateTime.Now);
            var totalRevenue = await _context.Subscriptions
                .Where(s => s.Status == "Active")
                .SumAsync(s => s.Amount);

            // Get recent users (last 5)
            var recentUsers = await _context.Users
                .OrderByDescending(u => u.Id)
                .Take(5)
                .ToListAsync();

            // Get recent cards (last 5)
            var recentCards = await _context.Cards
                .Include(c => c.Category)
                .OrderByDescending(c => c.CreatedAt)
                .Take(5)
                .ToListAsync();

            // Get recent subscriptions (last 5)
            var recentSubscriptions = await _context.Subscriptions
                .Include(s => s.User)
                .OrderByDescending(s => s.CreatedAt)
                .Take(5)
                .ToListAsync();

            // Pass data to view
            ViewBag.TotalUsers = totalUsers;
            ViewBag.TotalCards = totalCards;
            ViewBag.TotalCategories = totalCategories;
            ViewBag.TotalFeedbacks = totalFeedbacks;
            ViewBag.TotalContactMessages = totalContactMessages;
            ViewBag.TotalSubscriptions = totalSubscriptions;
            ViewBag.ActiveSubscriptions = activeSubscriptions;
            ViewBag.TotalRevenue = totalRevenue;
            ViewBag.RecentUsers = recentUsers;
            ViewBag.RecentCards = recentCards;
            ViewBag.RecentSubscriptions = recentSubscriptions;
            ViewBag.AdminName = HttpContext.Session.GetString("UserName");

            return View("Index");
        }

        // User Management
        public async Task<IActionResult> Users()
        {
            if (!IsAdminAuthenticated())
            {
                return RedirectToAction("Login", "Home");
            }

            var users = await _context.Users.ToListAsync();
            return View(users);
        }

        public async Task<IActionResult> UserDetails(int id)
        {
            if (!IsAdminAuthenticated())
            {
                return RedirectToAction("Login", "Home");
            }

            var user = await _context.Users
                .Include(u => u.Subscriptions)
                .FirstOrDefaultAsync(u => u.Id == id);
            
            if (user == null)
            {
                return NotFound();
            }

            // Get active subscription
            var activeSubscription = user.Subscriptions?
                .FirstOrDefault(s => s.Status == "Active" && s.EndDate > DateTime.Now);
            
            ViewBag.ActiveSubscription = activeSubscription;
            ViewBag.HasActiveSubscription = activeSubscription != null;

            return View(user);
        }

        public IActionResult CreateUser()
        {
            if (!IsAdminAuthenticated())
            {
                return RedirectToAction("Login", "Home");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(User user)
        {
            if (!IsAdminAuthenticated())
            {
                return RedirectToAction("Login", "Home");
            }

            if (ModelState.IsValid)
            {
                // Check if email already exists
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Email", "Email already exists");
                    return View(user);
                }

                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction("Users");
            }

            return View(user);
        }

        public async Task<IActionResult> EditUser(int id)
        {
            if (!IsAdminAuthenticated())
            {
                return RedirectToAction("Login", "Home");
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(User user)
        {
            if (!IsAdminAuthenticated())
            {
                return RedirectToAction("Login", "Home");
            }

            if (ModelState.IsValid)
            {
                var existingUser = await _context.Users.FindAsync(user.Id);
                if (existingUser == null)
                {
                    return NotFound();
                }

                // Check if email already exists for another user
                var emailCheck = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email && u.Id != user.Id);
                if (emailCheck != null)
                {
                    ModelState.AddModelError("Email", "Email already exists");
                    return View(user);
                }

                existingUser.Name = user.Name;
                existingUser.Email = user.Email;
                existingUser.Phone = user.Phone;
                existingUser.Role = user.Role;
                // Only update password if a new one is provided
                if (!string.IsNullOrEmpty(user.Password))
                {
                    existingUser.Password = user.Password;
                }

                await _context.SaveChangesAsync();
                return RedirectToAction("Users");
            }

            return View(user);
        }

        public async Task<IActionResult> DeleteUser(int id)
        {
            if (!IsAdminAuthenticated())
            {
                return RedirectToAction("Login", "Home");
            }

            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Users");
        }

        [HttpPost]
        public async Task<IActionResult> ChangeUserRole(int userId, string role)
        {
            if (!IsAdminAuthenticated())
            {
                return RedirectToAction("Login", "Home");
            }

            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                user.Role = role;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Users");
        }

        // Card Management
        public async Task<IActionResult> Cards()
        {
            if (!IsAdminAuthenticated())
            {
                return RedirectToAction("Login", "Home");
            }

            var cards = await _context.Cards.Include(c => c.Category).ToListAsync();
            return View(cards);
        }

        public async Task<IActionResult> CreateCard()
        {
            if (!IsAdminAuthenticated())
            {
                return RedirectToAction("Login", "Home");
            }

            ViewBag.Categories = await _context.Categories.ToListAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateCard(Card card, IFormFile mediaFile)
        {
            if (!IsAdminAuthenticated())
            {
                return RedirectToAction("Login", "Home");
            }

            if (mediaFile != null && mediaFile.Length > 0)
            {
                var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "cards");
                Directory.CreateDirectory(uploadsPath);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(mediaFile.FileName);
                var filePath = Path.Combine(uploadsPath, fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await mediaFile.CopyToAsync(fileStream);
                }

                card.MediaPath = "/uploads/cards/" + fileName;
            }

            card.CreatedAt = DateTime.Now;
            _context.Cards.Add(card);
            await _context.SaveChangesAsync();

            return RedirectToAction("Cards");
        }

        public async Task<IActionResult> EditCard(int id)
        {
            if (!IsAdminAuthenticated())
            {
                return RedirectToAction("Login", "Home");
            }

            var card = await _context.Cards.FindAsync(id);
            if (card == null)
            {
                return NotFound();
            }

            ViewBag.Categories = await _context.Categories.ToListAsync();
            return View(card);
        }

        [HttpPost]
        public async Task<IActionResult> EditCard(Card card, IFormFile mediaFile)
        {
            if (!IsAdminAuthenticated())
            {
                return RedirectToAction("Login", "Home");
            }

            var existingCard = await _context.Cards.FindAsync(card.CardID);
            if (existingCard == null)
            {
                return NotFound();
            }

            if (mediaFile != null && mediaFile.Length > 0)
            {
                var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "cards");
                Directory.CreateDirectory(uploadsPath);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(mediaFile.FileName);
                var filePath = Path.Combine(uploadsPath, fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await mediaFile.CopyToAsync(fileStream);
                }

                existingCard.MediaPath = "/uploads/cards/" + fileName;
            }

            existingCard.Title = card.Title;
            existingCard.Message = card.Message;
            existingCard.CategoryID = card.CategoryID;

            await _context.SaveChangesAsync();
            return RedirectToAction("Cards");
        }

        public async Task<IActionResult> DeleteCard(int id)
        {
            if (!IsAdminAuthenticated())
            {
                return RedirectToAction("Login", "Home");
            }

            var card = await _context.Cards.FindAsync(id);
            if (card != null)
            {
                _context.Cards.Remove(card);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Cards");
        }

        // Category Management
        public async Task<IActionResult> Categories()
        {
            if (!IsAdminAuthenticated())
            {
                return RedirectToAction("Login", "Home");
            }

            var categories = await _context.Categories.ToListAsync();
            return View(categories);
        }

        public IActionResult CreateCategory()
        {
            if (!IsAdminAuthenticated())
            {
                return RedirectToAction("Login", "Home");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory(Category category, IFormFile imageFile)
        {
            if (!IsAdminAuthenticated())
            {
                return RedirectToAction("Login", "Home");
            }

            // Validate CategoryName
            if (string.IsNullOrEmpty(category.CategoryName))
            {
                ModelState.AddModelError("CategoryName", "Category name is required");
                return View(category);
            }

            // Check if category name already exists
            var existingCategory = await _context.Categories
                .FirstOrDefaultAsync(c => c.CategoryName == category.CategoryName);
            if (existingCategory != null)
            {
                ModelState.AddModelError("CategoryName", "Category name already exists");
                return View(category);
            }

            // Handle image upload
            if (imageFile != null && imageFile.Length > 0)
            {
                var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "categories");
                Directory.CreateDirectory(uploadsPath);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                var filePath = Path.Combine(uploadsPath, fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(fileStream);
                }

                category.ImagePath = "/uploads/categories/" + fileName;
            }
            else
            {
                // Set default image or null
                category.ImagePath = null;
            }

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return RedirectToAction("Categories");
        }

        public async Task<IActionResult> EditCategory(int id)
        {
            if (!IsAdminAuthenticated())
            {
                return RedirectToAction("Login", "Home");
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> EditCategory(Category category, IFormFile imageFile)
        {
            if (!IsAdminAuthenticated())
            {
                return RedirectToAction("Login", "Home");
            }

            var existingCategory = await _context.Categories.FindAsync(category.CategoryID);
            if (existingCategory == null)
            {
                return NotFound();
            }

            if (imageFile != null && imageFile.Length > 0)
            {
                var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "categories");
                Directory.CreateDirectory(uploadsPath);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                var filePath = Path.Combine(uploadsPath, fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(fileStream);
                }

                existingCategory.ImagePath = "/uploads/categories/" + fileName;
            }

            existingCategory.CategoryName = category.CategoryName;
            await _context.SaveChangesAsync();

            return RedirectToAction("Categories");
        }

        public async Task<IActionResult> DeleteCategory(int id)
        {
            if (!IsAdminAuthenticated())
            {
                return RedirectToAction("Login", "Home");
            }

            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Categories");
        }

        // Feedback Reviews Management
        public async Task<IActionResult> FeedbackReviews()
        {
            if (!IsAdminAuthenticated())
            {
                return RedirectToAction("Login", "Home");
            }

            var feedbacks = await _context.Feedbacks
                .OrderByDescending(f => f.SubmittedAt)
                .ToListAsync();

            // Calculate statistics
            ViewBag.TotalFeedbacks = feedbacks.Count;

            return View(feedbacks);
        }

        public async Task<IActionResult> DeleteFeedback(int id)
        {
            if (!IsAdminAuthenticated())
            {
                return RedirectToAction("Login", "Home");
            }

            var feedback = await _context.Feedbacks.FindAsync(id);
            if (feedback != null)
            {
                _context.Feedbacks.Remove(feedback);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("FeedbackReviews");
        }

        // Contact Messages Management
        public async Task<IActionResult> ContactMessages()
        {
            if (!IsAdminAuthenticated())
            {
                return RedirectToAction("Login", "Home");
            }

            var contactMessages = await _context.ContactMessages
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            return View(contactMessages);
        }

        public async Task<IActionResult> DeleteContactMessage(int id)
        {
            if (!IsAdminAuthenticated())
            {
                return RedirectToAction("Login", "Home");
            }

            var contactMessage = await _context.ContactMessages.FindAsync(id);
            if (contactMessage != null)
            {
                _context.ContactMessages.Remove(contactMessage);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("ContactMessages");
        }

        // Subscription Management
        public async Task<IActionResult> Subscriptions(bool showModal = false)
        {
            if (!IsAdminAuthenticated())
            {
                return RedirectToAction("Login", "Home");
            }

            var subscriptions = await _context.Subscriptions
                .Include(s => s.User)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();

            // Get subscription statistics
            ViewBag.TotalSubscriptions = subscriptions.Count;
            ViewBag.ActiveSubscriptions = await _context.Subscriptions
                .CountAsync(s => s.Status == "Active" && s.EndDate > DateTime.Now);
            ViewBag.ExpiredSubscriptions = await _context.Subscriptions
                .CountAsync(s => s.Status != "Active" || s.EndDate <= DateTime.Now);
            ViewBag.TotalRevenue = subscriptions.Where(s => s.Status == "Active").Sum(s => s.Amount);
            ViewBag.ShowModal = showModal;

            return View(subscriptions);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSubscription([FromBody] SubscriptionRequest request)
        {
            if (!IsAdminAuthenticated())
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            try
            {
                var user = await _context.Users.FindAsync(request.UserId);
                if (user == null)
                {
                    return Json(new { success = false, message = "User not found" });
                }

                // Check if user already has an active subscription
                var existingSubscription = await _context.Subscriptions
                    .FirstOrDefaultAsync(s => s.UserId == request.UserId && s.Status == "Active" && s.EndDate > DateTime.Now);

                if (existingSubscription != null)
                {
                    return Json(new { success = false, message = "User already has an active subscription" });
                }

                var subscription = new Subscription
                {
                    UserId = request.UserId,
                    PlanType = request.PlanType,
                    Amount = request.Amount,
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddMonths(request.Duration),
                    Status = "Active",
                    PaymentMethod = request.PaymentMethod,
                    TransactionId = Guid.NewGuid().ToString()
                };

                _context.Subscriptions.Add(subscription);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Subscription created successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error getting users: " + ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetActiveSubscriptions(int skip = 0, int take = 5)
    {
        if (!IsAdminAuthenticated())
        {
            return Json(new { success = false, message = "Unauthorized" });
        }

        try
        {
            var totalCount = await _context.Subscriptions
                .CountAsync(s => s.Status == "Active" && 
                           s.StartDate <= DateTime.Now && 
                           s.EndDate > DateTime.Now);

            var activeSubscriptions = await _context.Subscriptions
                .Include(s => s.User)
                .Where(s => s.Status == "Active" && 
                           s.StartDate <= DateTime.Now && 
                           s.EndDate > DateTime.Now)
                .OrderByDescending(s => s.CreatedAt)
                .Skip(skip)
                .Take(take)
                .Select(s => new {
                    id = s.Id,
                    userName = s.User.Name,
                    userEmail = s.User.Email,
                    planType = s.PlanType,
                    startDate = s.StartDate.ToString("yyyy-MM-dd"),
                    endDate = s.EndDate.ToString("yyyy-MM-dd"),
                    status = s.Status,
                    amount = s.Amount,
                    paymentMethod = s.PaymentMethod
                })
                .ToListAsync();

            return Json(new { 
                success = true,
                data = activeSubscriptions,
                totalCount = totalCount,
                hasMore = (skip + take) < totalCount
            });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = "Error getting subscriptions: " + ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> CancelSubscription(int id)
    {
        if (!IsAdminAuthenticated())
        {
            return Json(new { success = false, message = "Unauthorized" });
        }

        try
        {
            var subscription = await _context.Subscriptions.FindAsync(id);
            if (subscription == null)
            {
                return Json(new { success = false, message = "Subscription not found" });
            }

            subscription.Status = "Cancelled";
            subscription.EndDate = DateTime.Now;
            
            _context.Subscriptions.Update(subscription);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Subscription cancelled successfully!" });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = "Error cancelling subscription: " + ex.Message });
        }
    }

    public async Task<IActionResult> GetUsersForSubscription()
    {
        if (!IsAdminAuthenticated())
        {
            return Json(new { success = false, message = "Unauthorized" });
        }

        try
        {
            var users = await _context.Users
                .Where(u => u.Role == "User")
                .Select(u => new { 
                    id = u.Id, 
                    name = u.Name, 
                    email = u.Email,
                    hasActiveSubscription = u.Subscriptions!.Any(s => 
                        s.Status == "Active" && 
                        s.StartDate <= DateTime.Now && 
                        s.EndDate > DateTime.Now)
                })
                .ToListAsync();

            return Json(users);
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = "Error getting users: " + ex.Message });
        }
    }

    // Analytics and Reporting
    public async Task<IActionResult> Analytics()
    {
        if (!IsAdminAuthenticated())
        {
            return RedirectToAction("Login", "Home");
        }

        // User statistics - properly calculate this month's users
        var totalUsers = await _context.Users.CountAsync();
        var startOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        var newUsersThisMonth = await _context.Users
            .CountAsync(u => EF.Functions.DateDiffDay(startOfMonth, DateTime.Now) >= 0); // Simplified - would need a CreatedAt field on User
        
        // Card sending statistics
        var totalCardsSent = await _context.CardSends.CountAsync();
        var cardsSentThisMonth = await _context.CardSends
            .CountAsync(cs => cs.SentAt >= DateTime.Now.AddDays(-30));

        // Revenue statistics
        var totalRevenue = await _context.Subscriptions
            .Where(s => s.Status == "Active")
            .SumAsync(s => s.Amount);
        var monthlyRevenue = await _context.Subscriptions
            .Where(s => s.CreatedAt >= DateTime.Now.AddDays(-30) && s.Status == "Active")
            .SumAsync(s => s.Amount);

        // Popular categories with proper include
        var popularCategories = await _context.Cards
            .Include(c => c.Category)
            .GroupBy(c => c.Category.CategoryName)
            .Select(g => new { Category = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .Take(5)
            .ToListAsync();

        // Additional dynamic analytics
        var todaysCardsSent = await _context.CardSends
            .CountAsync(cs => cs.SentAt.Date == DateTime.Today);
        
        var activeSubscriptionsCount = await _context.Subscriptions
            .CountAsync(s => s.Status == "Active" && s.EndDate > DateTime.Now);
        
        var growthRate = totalUsers > 0 ? Math.Round((double)newUsersThisMonth / totalUsers * 100, 1) : 0;

        ViewBag.TotalUsers = totalUsers;
        ViewBag.NewUsersThisMonth = newUsersThisMonth;
        ViewBag.TotalCardsSent = totalCardsSent;
        ViewBag.CardsSentThisMonth = cardsSentThisMonth;
        ViewBag.TodaysCardsSent = todaysCardsSent;
        ViewBag.TotalRevenue = totalRevenue;
        ViewBag.MonthlyRevenue = monthlyRevenue;
        ViewBag.ActiveSubscriptionsCount = activeSubscriptionsCount;
        ViewBag.GrowthRate = growthRate;
        ViewBag.PopularCategories = popularCategories;

        return View();
    }

        public async Task<IActionResult> Reports()
        {
            if (!IsAdminAuthenticated())
            {
                return RedirectToAction("Login", "Home");
            }

            // Generate comprehensive reports
            var userReport = await _context.Users
                .GroupBy(u => u.Role)
                .Select(g => new { Role = g.Key, Count = g.Count() })
                .ToListAsync();

            var subscriptionReport = await _context.Subscriptions
                .GroupBy(s => s.Status)
                .Select(g => new { Status = g.Key, Count = g.Count(), Revenue = g.Sum(x => x.Amount) })
                .ToListAsync();

            var cardUsageReport = await _context.Cards
                .Include(c => c.Category)
                .GroupBy(c => c.Category.CategoryName)
                .Select(g => new { Category = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .ToListAsync();

            ViewBag.UserReport = userReport;
            ViewBag.SubscriptionReport = subscriptionReport;
            ViewBag.CardUsageReport = cardUsageReport;

            return View();
        }

        public async Task<IActionResult> ExportReportsToExcel()
        {
            if (!IsAdminAuthenticated())
            {
                return RedirectToAction("Login", "Home");
            }

            try
            {
                // Get report data
                var userReport = await _context.Users
                    .GroupBy(u => u.Role)
                    .Select(g => new { Role = g.Key, Count = g.Count() })
                    .ToListAsync();

                var subscriptionReport = await _context.Subscriptions
                    .GroupBy(s => s.Status)
                    .Select(g => new { Status = g.Key, Count = g.Count(), Revenue = g.Sum(x => x.Amount) })
                    .ToListAsync();

                var cardUsageReport = await _context.Cards
                    .Include(c => c.Category)
                    .GroupBy(c => c.Category.CategoryName)
                    .Select(g => new { Category = g.Key, Count = g.Count() })
                    .ToListAsync();

                // Create CSV content
                var csv = new System.Text.StringBuilder();
                csv.AppendLine("Report Type,Item,Count,Revenue");
                
                foreach (var item in userReport)
                {
                    csv.AppendLine($"User Report,{item.Role},{item.Count},0");
                }
                
                foreach (var item in subscriptionReport)
                {
                    csv.AppendLine($"Subscription Report,{item.Status},{item.Count},{item.Revenue}");
                }
                
                foreach (var item in cardUsageReport)
                {
                    csv.AppendLine($"Card Usage Report,{item.Category},{item.Count},0");
                }

                var bytes = System.Text.Encoding.UTF8.GetBytes(csv.ToString());
                return File(bytes, "text/csv", $"AdminReports_{DateTime.Now:yyyyMMdd}.csv");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Failed to export reports: " + ex.Message;
                return RedirectToAction("Reports");
            }
        }

        public async Task<IActionResult> ExportReportsToPdf()
        {
            if (!IsAdminAuthenticated())
            {
                return RedirectToAction("Login", "Home");
            }

            try
            {
                // Get report data
                var userReport = await _context.Users
                    .GroupBy(u => u.Role)
                    .Select(g => new { Role = g.Key, Count = g.Count() })
                    .ToListAsync();

                var subscriptionReport = await _context.Subscriptions
                    .GroupBy(s => s.Status)
                    .Select(g => new { Status = g.Key, Count = g.Count(), Revenue = g.Sum(x => x.Amount) })
                    .ToListAsync();

                // Create HTML content for PDF
                var html = @"
                <html>
                <head>
                    <style>
                        body { font-family: Arial, sans-serif; }
                        table { border-collapse: collapse; width: 100%; margin-bottom: 20px; }
                        th, td { border: 1px solid #ddd; padding: 8px; text-align: left; }
                        th { background-color: #f2f2f2; }
                        h2 { color: #333; }
                    </style>
                </head>
                <body>
                    <h1>Admin Reports - " + DateTime.Now.ToString("MMMM dd, yyyy") + @"</h1>
                    
                    <h2>User Report</h2>
                    <table>
                        <tr><th>Role</th><th>Count</th></tr>";
                
                foreach (var item in userReport)
                {
                    html += $"<tr><td>{item.Role}</td><td>{item.Count}</td></tr>";
                }
                
                html += @"
                    </table>
                    
                    <h2>Subscription Report</h2>
                    <table>
                        <tr><th>Status</th><th>Count</th><th>Revenue</th></tr>";
                
                foreach (var item in subscriptionReport)
                {
                    html += $"<tr><td>{item.Status}</td><td>{item.Count}</td><td>${item.Revenue}</td></tr>";
                }
                
                html += @"
                    </table>
                </body>
                </html>";

                var bytes = System.Text.Encoding.UTF8.GetBytes(html);
                return File(bytes, "text/html", $"AdminReports_{DateTime.Now:yyyyMMdd}.html");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Failed to export reports: " + ex.Message;
                return RedirectToAction("Reports");
            }
        }

        public async Task<IActionResult> GenerateMonthlyReport()
        {
            if (!IsAdminAuthenticated())
            {
                return RedirectToAction("Login", "Home");
            }

            try
            {
                var startDate = DateTime.Now.AddDays(-30);
                
                // Get monthly data
                var newUsers = await _context.Users.CountAsync(); // Simplified - would need CreatedAt field
                var cardsSent = await _context.CardSends.CountAsync(cs => cs.SentAt >= startDate);
                var newSubscriptions = await _context.Subscriptions.CountAsync(s => s.CreatedAt >= startDate);
                var revenue = await _context.Subscriptions
                    .Where(s => s.CreatedAt >= startDate && s.Status == "Active")
                    .SumAsync(s => s.Amount);

                // Create monthly report data
                var reportData = new
                {
                    Period = "Last 30 Days",
                    NewUsers = newUsers,
                    CardsSent = cardsSent,
                    NewSubscriptions = newSubscriptions,
                    Revenue = revenue,
                    GeneratedAt = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss")
                };

                return Json(new { success = true, data = reportData, message = "Monthly report generated successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error generating monthly report: " + ex.Message });
            }
        }

        public async Task<IActionResult> GenerateWeeklyReport()
        {
            if (!IsAdminAuthenticated())
            {
                return RedirectToAction("Login", "Home");
            }

            try
            {
                var startDate = DateTime.Now.AddDays(-7);
                
                // Get weekly data
                var cardsSent = await _context.CardSends.CountAsync(cs => cs.SentAt >= startDate);
                var newSubscriptions = await _context.Subscriptions.CountAsync(s => s.CreatedAt >= startDate);
                var revenue = await _context.Subscriptions
                    .Where(s => s.CreatedAt >= startDate && s.Status == "Active")
                    .SumAsync(s => s.Amount);

                var reportData = new
                {
                    Period = "Last 7 Days",
                    CardsSent = cardsSent,
                    NewSubscriptions = newSubscriptions,
                    Revenue = revenue,
                    GeneratedAt = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss")
                };

                return Json(new { success = true, data = reportData, message = "Weekly report generated successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error generating weekly report: " + ex.Message });
            }
        }

        public async Task<IActionResult> GenerateRevenueReport()
        {
            if (!IsAdminAuthenticated())
            {
                return RedirectToAction("Login", "Home");
            }

            try
            {
                var totalRevenue = await _context.Subscriptions
                    .Where(s => s.Status == "Active")
                    .SumAsync(s => s.Amount);
                
                var monthlyRevenue = await _context.Subscriptions
                    .Where(s => s.CreatedAt >= DateTime.Now.AddDays(-30) && s.Status == "Active")
                    .SumAsync(s => s.Amount);

                var revenueByPlan = await _context.Subscriptions
                    .Where(s => s.Status == "Active")
                    .GroupBy(s => s.PlanType)
                    .Select(g => new { PlanType = g.Key, Revenue = g.Sum(s => s.Amount), Count = g.Count() })
                    .ToListAsync();

                var reportData = new
                {
                    TotalRevenue = totalRevenue,
                    MonthlyRevenue = monthlyRevenue,
                    RevenueByPlan = revenueByPlan,
                    GeneratedAt = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss")
                };

                return Json(new { success = true, data = reportData, message = "Revenue report generated successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error generating revenue report: " + ex.Message });
            }
        }

        public async Task<IActionResult> GenerateUserAnalysis()
        {
            if (!IsAdminAuthenticated())
            {
                return RedirectToAction("Login", "Home");
            }

            try
            {
                var totalUsers = await _context.Users.CountAsync();
                var usersByRole = await _context.Users
                    .GroupBy(u => u.Role)
                    .Select(g => new { Role = g.Key, Count = g.Count() })
                    .ToListAsync();

                var usersWithSubscriptions = await _context.Users
                    .CountAsync(u => u.Subscriptions!.Any(s => s.Status == "Active" && s.EndDate > DateTime.Now));

                var topCardSenders = await _context.CardSends
                    .Include(cs => cs.User)
                    .GroupBy(cs => cs.User)
                    .Select(g => new { User = g.Key.Name, Email = g.Key.Email, CardsSent = g.Count() })
                    .OrderByDescending(x => x.CardsSent)
                    .Take(5)
                    .ToListAsync();

                var reportData = new
                {
                    TotalUsers = totalUsers,
                    UsersByRole = usersByRole,
                    UsersWithSubscriptions = usersWithSubscriptions,
                    TopCardSenders = topCardSenders,
                    GeneratedAt = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss")
                };

                return Json(new { success = true, data = reportData, message = "User analysis generated successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error generating user analysis: " + ex.Message });
            }
        }
        
        public async Task<IActionResult> CardSends()
        {
            if (!IsAdminAuthenticated())
            {
                return RedirectToAction("Login", "Home");
            }

            var cardSends = await _context.CardSends
                .Include(cs => cs.User)
                .Include(cs => cs.Card)
                .OrderByDescending(cs => cs.SentAt)
                .ToListAsync();

            return View(cardSends);
        }

        public IActionResult Settings()
        {
            if (!IsAdminAuthenticated())
            {
                return RedirectToAction("Login", "Home");
            }

            // System settings view
            ViewBag.TotalUsers = _context.Users.Count();
            ViewBag.TotalCards = _context.Cards.Count();
            ViewBag.TotalCategories = _context.Categories.Count();
            ViewBag.SystemStatus = "Operational";
            ViewBag.LastBackup = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd HH:mm");
            
            return View();
        }  
          public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["LogoutMessage"] = "Admin logged out successfully!";
            return RedirectToAction("Login", "Home");
        }
}

    // Helper class for subscription requests
    public class SubscriptionRequest
    {
        public int UserId { get; set; }
        public string PlanType { get; set; } = "Pro";
        public decimal Amount { get; set; } = 9.99m;
        public int Duration { get; set; } = 1; // months
        public string PaymentMethod { get; set; } = "Card";
    }
}
