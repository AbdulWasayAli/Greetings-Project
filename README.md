# E-Project (Greeting Cards Web App)

## Overview
E-Project is an **ASP.NET Core MVC** web application for **Greeting Cards**.

It includes:
- Category browsing (users can search categories and view cards)
- **Pro subscription** system (users can send greeting cards via email)
- Admin dashboard to manage content and view analytics/reporting

Core parts:
- Email sending (SMTP)
- Session-based authentication
- SQL Server database (EF Core)
- Initial seed data (SeedData)


---

## Tech Stack
- **Backend:** ASP.NET Core MVC
- **Database:** SQL Server (Entity Framework Core)
- **ORM:** Entity Framework Core (Code First + Migrations)
- **Session Auth:** `HttpContext.Session` (UserName/UserRole)
- **File Upload / Media:** `wwwroot/uploads/*`
- **Email:** SMTP (Gmail SMTP usage code HomeController.cs me)

---

## Project Structure (important folders)
- `E-Project/Program.cs`
  - Services register
  - Session enable
  - AVIF content-type support
  - `SeedData.Initialize()` call
- `E-Project/Data/ApplicationDbContext.cs`
  - DbSets: `Users, Categories, Cards, Subscriptions, CardSends, Feedbacks, ContactMessages`
  - Relationships configure
- `E-Project/Data/SeedData.cs`
  - Categories + Cards seed karta hai (agar database empty ho)
- `E-Project/Controllers/HomeController.cs`
  - Main site routes: Index/Cards/About/Contact/Feedback/Login/Register/Subscribe/SendCard
- `E-Project/Controllers/AdminController.cs`
  - Admin features: Dashboard, Users, Cards, Categories, FeedbackReviews, ContactMessages, Subscriptions, Analytics, Reports
- `E-Project/Migrations/`
  - DB schema evolution
- `E-Project/wwwroot/`
  - Static assets + uploads + images

---

## Key Features
### 1) Authentication & Authorization
- Login: `HomeController.Login(email, password)`
- Session keys:
  - `UserName`
  - `UserRole`
- Admin routes `AdminController` me protected hain.
- Admin navigation lock middleware:
  - `E-Project/Filters/PreventAdminAccessToMainSiteFilter.cs`
  - `E-Project/Middleware/AdminNavigationLockMiddleware.cs`

### 2) Categories & Cards
- `Home/Index`: Search ke through category redirect + recent feedback show
- `Home/Cards/{id}`: Category ke cards show
- SeedData me initial categories/cards set hain (Birthday/Wedding/New Year/...)

### 3) Subscription System (Pro)
- Pro subscription required for card sending
- Subscription model me fields:
  - `PlanType`, `Amount`, `StartDate`, `EndDate`, `Status`, `PaymentMethod`, `TransactionId`
- Admin side me subscription listing + cancel endpoints.

### 4) Send Greeting Cards via Email
- `HomeController.SendCard` page (GET) aur API action (POST)
- Daily limit logic (Pro plan): code me **10 emails/day** limit mention hai.
- Image handling:
  - Template image (User/images/...) embed hoti hai
  - Custom base64 image ko `wwwroot/uploads/*` me save karke embed/attach kiya jata hai
- Email HTML styling `GenerateStyledEmailHtml()` function me generate hoti hai

### 5) Admin Dashboard & Reporting
- Dashboard statistics:
  - total users/cards/categories/feedback/contact messages
  - active subscriptions count + revenue
- Analytics:
  - card sends, revenue, popular categories, etc.
- Reports:
  - Excel export (CSV format in current code)
  - PDF export (HTML returned as `text/html` in current code)

---

## Database Setup (SQL Server + EF Core)
### Connection String
`E-Project/appsettings.json` me connection string:
- `ConnectionStrings:DefaultConnection`

Default example (repo me):
- `Server=DESKTOP-9V3MHPU; Database=E-Project; Trusted_Connection=True; TrustServerCertificate=True;`

> Aapko apne SQL Server details ke hisaab se `appsettings.json` update karni hogi.

### Migrations
Migrations `E-Project/Migrations/` folder me stored hain.

Common commands (Package Manager Console ya terminal me):
```bash
dotnet ef migrations add InitialCreate
 dotnet ef database update
```

---

## How to Run
### Prerequisites
- .NET SDK installed
- SQL Server installed aur running

### Steps
1) Repo open karo: `E-Project.sln`
2) `E-Project/appsettings.json` me DB connection verify karo
3) Database update karo (agar required):
   ```bash
   dotnet ef database update
   ```
4) App run karo:
   ```bash
   dotnet run --project E-Project
   ```
5) Browser me open:
   - default MVC route: `/{controller}/{action}`
   - Program.cs me default route: `{controller=Home}/{action=Index}/{id?}`

---

## Admin / User Flow
### Login
- User role ke basis pe redirect:
  - Admin -> `AdminController.Dashboard`
  - User -> `HomeController.Index`

### Admin Actions
- Manage Users/Cards/Categories
- Delete feedback/contact messages
- Subscriptions list + cancel
- Analytics & reports

---

## Important Notes (Security / Email)
- **Passwords are stored in DB as plain text** (HomeController/AdminController me direct equality check show hota hai). 
  - Real production me password hashing recommend hai.
- Email SMTP credentials code me directly present hain.
  - Production me secrets/ENV variables + secure secret store use karein.

---

## Media & Uploads
- Cards uploads path (admin):
  - `wwwroot/uploads/cards/`
- Category images uploads path (admin):
  - `wwwroot/uploads/categories/`
- Custom images (SendCard):
  - `wwwroot/uploads/`
- AVIF content-type support Program.cs me added hai.

---

## Seed Data
`SeedData.Initialize()` database empty hone par:
- Categories seed karta hai (Birthday, Wedding, New Year, Eid-ul-Fitr, Eid-ul-Adha, Christmas, Thanksgiving, Diwali)
- Cards seed karta hai (CategoryID mapping ke saath)

---

## License
Project license file repo me specify nahi hui—agar required ho to apni license add kar dein.

