using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace E_Project.Filters
{
    public class PreventAdminAccessToMainSiteFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var session = context.HttpContext.Session;
            var userRole = session.GetString("UserRole");
            var userName = session.GetString("UserName");
            
            // If user is logged in as Admin, prevent access to main site
            if (!string.IsNullOrEmpty(userName) && userRole == "Admin")
            {
                var controllerName = context.RouteData.Values["controller"]?.ToString();
                var actionName = context.RouteData.Values["action"]?.ToString();
                
                // Allow access only to Logout action for admin logout
                if (controllerName?.ToLower() == "home" && actionName?.ToLower() == "logout")
                {
                    base.OnActionExecuting(context);
                    return;
                }
                
                // Redirect admin back to dashboard for any other main site access
                if (controllerName?.ToLower() != "admin")
                {
                    // Set cache control headers to prevent caching
                    context.HttpContext.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
                    context.HttpContext.Response.Headers["Pragma"] = "no-cache";
                    context.HttpContext.Response.Headers["Expires"] = "0";
                    
                    // Add a message to inform the admin
                    context.HttpContext.Session.SetString("AdminNavigationMessage", 
                        "Access Restricted: You are logged in as Admin. Please logout first to access the main website.");
                    
                    context.Result = new RedirectToActionResult("Dashboard", "Admin", null);
                    return;
                }
            }
            
            base.OnActionExecuting(context);
        }
    }
}