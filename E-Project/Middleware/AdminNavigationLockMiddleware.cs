using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace E_Project.Middleware
{
    public class AdminNavigationLockMiddleware
    {
        private readonly RequestDelegate _next;

        public AdminNavigationLockMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var session = context.Session;
            var userRole = session.GetString("UserRole");
            var userName = session.GetString("UserName");

            // Check if user is logged in as admin
            if (!string.IsNullOrEmpty(userName) && userRole == "Admin")
            {
                var path = context.Request.Path.Value?.ToLower() ?? "";
                var isAdminPath = path.StartsWith("/admin");
                var isLogoutPath = path.Equals("/home/logout", StringComparison.OrdinalIgnoreCase);

                // If admin is trying to access non-admin pages (except logout)
                if (!isAdminPath && !isLogoutPath && !path.StartsWith("/dashboard/assets"))
                {
                    // Set no-cache headers to prevent browser caching
                    context.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
                    context.Response.Headers["Pragma"] = "no-cache";
                    context.Response.Headers["Expires"] = "0";
                    context.Response.Headers["X-Content-Type-Options"] = "nosniff";
                    context.Response.Headers["X-Frame-Options"] = "DENY";

                    // Store message for dashboard display
                    session.SetString("AdminNavigationMessage", 
                        "Navigation blocked: You must logout before accessing the main website.");

                    // Redirect to admin dashboard
                    context.Response.Redirect("/Admin/Dashboard", permanent: false);
                    return;
                }
            }

            await _next(context);
        }
    }
}