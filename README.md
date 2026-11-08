# Greeting Cards

An interactive web application built with ASP.NET Core 8.0 MVC for customizing and sharing greeting cards across major occasions and festivals.

---

## Tech Stack & Versions

- **Backend Framework:** .NET 8.0 (ASP.NET Core MVC - `net8.0`)
- **ORM / Data Access:** Entity Framework Core `8.0.19`
- **Database Provider:** SQLite (`Microsoft.EntityFrameworkCore.Sqlite` `v8.0.19`)
- **Design Tools:** EF Core Tools & Design (`v8.0.19`)
- **Frontend Engine:** Razor Views (`.cshtml`), HTML5, CSS3, JavaScript (ES6)
- **UI Framework:** Bootstrap `v5.3`
- **Icons & Animations:** FontAwesome `v6.0`, AOS (Animate on Scroll)
- **Containerization:** Docker (Multi-stage build using `mcr.microsoft.com/dotnet/sdk:8.0` and `aspnet:8.0`)

---

## Features

- **Occasions & Festivals:** Dedicated card collections across Birthday, Wedding, New Year, Eid-ul-Fitr, Eid-ul-Adha, Christmas, Thanksgiving, and Diwali.
- **In-Browser Card Customizer:** Interactive modal enabling custom text, recipient information, personalized message crafting, and live previews.
- **Rich Media & Interactive UI:** Front-and-back flip animations on cards, responsive grid layout, and background video support on main pages.
- **Subscription Management:** Tiered access (Free and Pro) regulating card sending allowances.
- **Feedback & Ratings:** User reviews with interactive 1 to 5 star ratings.
- **Administrative Suite:** Dashboard for monitoring card categories, catalog management, feedback reviews, and platform analytics.

---

## Project Structure

```text
Greetings-Project/
├── Dockerfile                        # Multi-stage production container configuration
├── E-Project.sln                     # Visual Studio Solution File
├── README.md                         # Project documentation
│
└── E-Project/                        # Main ASP.NET Core MVC Application
    ├── appsettings.json              # App configuration & database connection string
    ├── E-Project.csproj              # Package references and target framework (.NET 8.0)
    ├── Program.cs                    # Application setup, middleware pipeline & service configuration
    │
    ├── Controllers/
    │   ├── HomeController.cs         # Public user workflows (Cards, Customizer, Feedback, Auth)
    │   └── AdminController.cs        # Administrative operations (Dashboard, Reports, Analytics)
    │
    ├── Data/
    │   ├── ApplicationDbContext.cs   # Entity Framework database context
    │   └── SeedData.cs               # Initial catalog seeding logic
    │
    ├── Models/                       # Domain models (Card, Category, User, Subscription, Feedback, etc.)
    ├── Views/                        # Razor Views for Home and Admin modules
    └── wwwroot/                      # Static client assets (CSS, JS, card images, and background videos)
```

---

## Running Locally

### Prerequisites
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) installed on your system.

### Steps

1. Clone the repository:
   ```bash
   git clone https://github.com/AbdulWasayAli/Greetings-Project.git
   cd Greetings-Project/E-Project
   ```

2. Restore dependencies and run the application:
   ```bash
   dotnet restore
   dotnet run
   ```

3. Open your browser and navigate to:
   ```text
   http://localhost:5118
   ```

---

## Deployment

A production-ready `Dockerfile` is included in the project root for continuous deployment on platforms such as Render, Railway, or Azure App Service.
