# Greeting Cards - Spreading Smiles With Every Card

A comprehensive, full-featured ASP.NET Core 8.0 MVC web application designed for browsing, customizing, sending greetings cards, managing user subscriptions, and overseeing platform analytics via an administrative dashboard.

---

## Table of Contents
1. [Project Overview](#project-overview)
2. [Technology Stack & Versions](#technology-stack--versions)
3. [Key Features](#key-features)
   - [User Facing](#user-facing)
   - [Admin Panel](#admin-panel)
4. [Folder Structure](#folder-structure)
5. [Database Schema & Architecture](#database-schema--architecture)
6. [Getting Started Locally](#getting-started-locally)
   - [Prerequisites](#prerequisites)
   - [Step-by-Step Execution](#step-by-step-execution)
7. [Default Credentials](#default-credentials)
8. [Docker & Production Deployment](#docker--production-deployment)
9. [Contributing & License](#contributing--license)

---

## Project Overview

**Greeting Cards** is an e-greetings platform providing users with hundreds of customized cards across major cultural occasions and religious festivals. The application supports interactive front-and-back card flips, in-browser personalization (custom titles, messages, recipient information, photo previews), instant card sharing/sending, user subscription management, rating/feedback collection, and a full-featured admin management suite with live analytics and report export.

---

## Technology Stack & Versions

| Component | Framework / Tool | Version | Description |
| :--- | :--- | :--- | :--- |
| **Backend Framework** | .NET (ASP.NET Core MVC) | `8.0` (`net8.0`) | High-performance cross-platform server |
| **ORM / Data Access** | Entity Framework Core | `8.0.19` | Database abstraction & LINQ querying |
| **Database Provider** | SQLite | `8.0.19` | Zero-configuration local database (`app.db`) |
| **SQL Server Support** | EF Core SqlServer (Compatible) | `8.0.19` | Optional SQL Server migration capability |
| **Frontend UI** | Razor Views (`.cshtml`), HTML5, CSS3, JS | Standard | Responsive mobile-first interface |
| **Styling & Icons** | Bootstrap 5, FontAwesome, AOS (Animate on Scroll) | Latest | Modern glassmorphic theme with animated video backgrounds |
| **Deployment Engine** | Docker & Render.com / Azure Web App | Multi-stage | Containerized production deployment |

---

## Key Features

### User Facing
- **Rich Media Hero & Auth Backgrounds:** Autoplay looped video backgrounds on Landing, Login, and Registration pages.
- **Card Categories (Occasions & Festivals):**
  - **Birthday** (12 Cards)
  - **Wedding** (12 Cards)
  - **New Year** (12 Cards)
  - **Eid-ul-Fitr** (12 Cards)
  - **Eid-ul-Adha** (12 Cards)
  - **Christmas** (12 Cards)
  - **Thanksgiving** (12 Cards)
  - **Diwali** (12 Cards)
- **Interactive Card Customizer:** Modal-driven customization enabling real-time preview, personalized greetings, title modifications, and custom recipient email dispatch.
- **Subscription Plans:** Free and Pro membership tiers governing sending quotas and access.
- **Feedback & Review System:** User submitted ratings (1-5 stars) and message reviews visible across the platform.
- **Contact Us & Support:** Dynamic message submission form with database tracking.

### Admin Panel
- **Comprehensive Dashboard (`/Admin/Dashboard`):** Instant statistics on active users, total categories, available cards, feedback counts, active subscriptions, and revenue.
- **Category & Card Management:** Add, edit, remove, and update media pathways for all categories and individual cards.
- **User Management (`/Admin/Users`):** User accounts monitoring, role assignment (Admin / User), and profile inspection.
- **Analytics & Graphs (`/Admin/Analytics`):** Trend charts for card sends, user acquisition, and engagement.
- **Reports & Data Export (`/Admin/Reports`):** Filtered reporting with one-click export to CSV and PDF.
- **Feedback Moderation (`/Admin/FeedbackReviews`):** Review user ratings, comments, and submission timestamps.

---

## Folder Structure

```text
Greetings-Project/
│
├── Dockerfile                        # Multi-stage production container configuration
├── E-Project.sln                     # Visual Studio Solution File
├── README.md                         # Comprehensive project documentation
│
├── Database/
│   └── project.bak                   # Original SQL Server database backup archive
│
└── E-Project/                        # Main ASP.NET Core MVC Application Root
    │
    ├── app.db                        # SQLite database file (auto-generated)
    ├── appsettings.json              # Connection strings and logging settings
    ├── appsettings.Development.json  # Development environment configuration
    ├── E-Project.csproj              # Project dependencies and TargetFramework definition
    ├── Program.cs                    # Application entry point, middleware & DI container
    │
    ├── Controllers/
    │   ├── HomeController.cs         # Public user flows (Cards, Login, Register, Feedback, etc.)
    │   └── AdminController.cs        # Administrative functions (Dashboard, Analytics, Reports, etc.)
    │
    ├── Data/
    │   ├── ApplicationDbContext.cs   # EF Core DbContext containing DbSets for all entities
    │   └── SeedData.cs               # Initial seeding logic for categories, 96 cards & accounts
    │
    ├── Filters/
    │   └── PreventAdminAccessToMainSiteFilter.cs   # Navigation route security filters
    │
    ├── Middleware/
    │   └── AdminNavigationLockMiddleware.cs        # Session lock validation middleware
    │
    ├── Migrations/                   # EF Core database migrations history
    │
    ├── Models/
    │   ├── Card.cs                   # Card entity (Title, MediaPath, CategoryID, etc.)
    │   ├── CardSend.cs               # Dispatch history log (RecipientEmail, Message, SentAt)
    │   ├── Category.cs               # Category entity (CategoryName, ImagePath)
    │   ├── Contact.cs                # Contact form model
    │   ├── ErrorViewModel.cs         # Generic error model
    │   ├── Feedback.cs               # User reviews and 1-5 star ratings
    │   ├── SendCardModel.cs          # Custom card dispatch payload model
    │   ├── Subscription.cs           # User plan subscription entity
    │   └── User.cs                   # User account entity (Name, Email, Password, Role)
    │
    ├── Properties/
    │   └── launchSettings.json       # Local development ports and IIS profile configurations
    │
    ├── Views/
    │   ├── _ViewImports.cshtml       # Global Razor tag helpers & namespaces
    │   ├── _ViewStart.cshtml         # Layout initialization
    │   ├── Home/                     # Public Razor views
    │   │   ├── About.cshtml
    │   │   ├── Cards.cshtml
    │   │   ├── Contact.cshtml
    │   │   ├── Feedback.cshtml
    │   │   ├── Index.cshtml          # Landing hero view with video backdrop
    │   │   ├── Login.cshtml          # Authentication view with video backdrop
    │   │   ├── Privacy.cshtml
    │   │   ├── Register.cshtml       # Signup view with video backdrop
    │   │   ├── SendCard.cshtml
    │   │   └── Subscribe.cshtml
    │   ├── Admin/                    # Administrative management views
    │   │   ├── Analytics.cshtml
    │   │   ├── Cards.cshtml
    │   │   ├── Categories.cshtml
    │   │   ├── ContactMessages.cshtml
    │   │   ├── CreateCard.cshtml
    │   │   ├── CreateCategory.cshtml
    │   │   ├── CreateUser.cshtml
    │   │   ├── EditCard.cshtml
    │   │   ├── EditCategory.cshtml
    │   │   ├── EditUser.cshtml
    │   │   ├── FeedbackReviews.cshtml
    │   │   ├── Index.cshtml
    │   │   ├── Reports.cshtml
    │   │   ├── Subscriptions.cshtml
    │   │   └── Users.cshtml
    │   └── Shared/                   # Shared templates, headers, footers & layouts
    │       ├── _AdminHeader.cshtml
    │       ├── _Footer.cshtml
    │       ├── _Header.cshtml
    │       ├── _Layout.cshtml        # Main user layout
    │       ├── _LayoutAdmin.cshtml   # Admin dashboard layout
    │       └── _SubHeader.cshtml
    │
    └── wwwroot/                      # Static client assets
        ├── css/                      # Custom stylesheets
        ├── js/                       # Client scripting & modals
        ├── lib/                      # Vendor libraries (Bootstrap, jQuery, etc.)
        └── User/
            └── images/               # Card media (8 categories x 12 cards), icons & video backgrounds
```

---

## Database Schema & Architecture

The application runs using **SQLite** (`app.db`) via Entity Framework Core, eliminating the need for an external database server during local development.

### Core Tables:
1. **`Users`**: Stores account profiles, passwords, contact phone numbers, and authorization roles (`Admin` or `User`).
2. **`Categories`**: Holds occasion and festival groupings along with header thumbnail images.
3. **`Cards`**: Stores individual greeting card designs with titles, sample messages, category relations, and asset file paths.
4. **`CardSends`**: Tracks card dispatch activity, recipient email, personalized greeting body, timestamps, and delivery status.
5. **`Subscriptions`**: Manages user subscription tiers (Free / Pro), payment status, and duration ranges.
6. **`Feedbacks`**: Stores visitor reviews, ratings (1-5 stars), email addresses, and submission dates.
7. **`ContactMessages`**: Logs inquiries submitted via the Contact form.

---

## Getting Started Locally

### Prerequisites
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) installed on your operating system.
- Terminal / PowerShell or any modern code editor (VS Code, Visual Studio, or Cursor).
- *No Visual Studio 2022 or Microsoft SQL Server installation is required.*

### Step-by-Step Execution

1. **Clone the repository:**
   ```bash
   git clone https://github.com/AbdulWasayAli/Greetings-Project.git
   cd Greetings-Project
   ```

2. **Navigate to the application folder:**
   ```bash
   cd E-Project
   ```

3. **Restore packages & build:**
   ```bash
   dotnet restore
   dotnet build
   ```

4. **Run the web application:**
   ```bash
   dotnet run
   ```

5. **Open in browser:**
   Open your browser and navigate to:
   ```text
   http://localhost:5118
   ```

*(On initial startup, `app.db` will be verified automatically, and default categories, cards, and admin accounts will be seeded seamlessly.)*

---

## Default Credentials

### Administrator Access
- **URL:** `http://localhost:5118/Home/Login`
- **Email:** `admin@greetings.com`
- **Password:** `admin123`
- **Dashboard Direct Link:** `http://localhost:5118/Admin/Dashboard`

### Regular User Access
- **Email:** `wasay3105@gmail.com`
- **Password:** `123456`

---

## Docker & Production Deployment

A pre-configured production Dockerfile is included at the project root for rapid deployment on **Render.com**, **Railway**, **Fly.io**, or **Microsoft Azure App Service**.

### Build and Run with Docker Locally:
```bash
# Build docker container
docker build -t greeting-cards .

# Run container on port 8080
docker run -d -p 8080:8080 --name greeting-cards-app greeting-cards
```
Access via `http://localhost:8080`.

### Deploying to Render.com:
1. Push your updated code to your GitHub repository.
2. In [Render.com](https://render.com), click **New +** -> **Web Service**.
3. Select your repository `AbdulWasayAli/Greetings-Project`.
4. Choose **Docker** as the environment runtime.
5. Click **Create Web Service** to start continuous deployment.

---

## Contributing & License

This project was created for educational and commercial greeting delivery purposes. Contributions, bug reports, and enhancements are welcome via GitHub Pull Requests.
