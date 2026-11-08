# Greeting Cards

An interactive web application built with ASP.NET Core 8.0 MVC for customizing and sharing greeting cards across major occasions and festivals.

---

## Tech Stack

- **Framework:** .NET 8.0 (ASP.NET Core MVC)
- **Database:** Entity Framework Core 8.0 with SQLite (`app.db`)
- **Frontend:** Razor Views, HTML5, CSS3, JavaScript
- **UI & Libraries:** Bootstrap 5, FontAwesome, AOS (Animate on Scroll)
- **Deployment:** Docker support for cloud hosting (Render, Azure, Railway)

---

## Features

- **Occasions & Festivals:** Dedicated categories including Birthday, Wedding, New Year, Eid-ul-Fitr, Eid-ul-Adha, Christmas, Thanksgiving, and Diwali.
- **Card Customization:** In-browser customization modal with real-time preview, personalized greetings, title editing, and direct sharing.
- **Interactive UI:** Front-and-back flip animation for cards, responsive layout, and video backgrounds on main pages.
- **Feedback & Reviews:** User feedback and star ratings system.
- **Admin Management:** Management interface for overseeing cards, categories, feedbacks, and activity analytics.

---

## Project Structure

```text
Greetings-Project/
├── Dockerfile                        # Container configuration for deployment
├── E-Project.sln                     # Visual Studio Solution File
├── README.md                         # Project documentation
│
└── E-Project/                        # Main Application
    ├── appsettings.json              # App configuration & connection string
    ├── E-Project.csproj              # Project dependencies & framework settings
    ├── Program.cs                    # Application setup & middleware
    │
    ├── Controllers/
    │   ├── HomeController.cs         # Public user flows & card actions
    │   └── AdminController.cs        # Administrative functions
    │
    ├── Data/
    │   ├── ApplicationDbContext.cs   # Entity Framework database context
    │   └── SeedData.cs               # Initial categories and cards setup
    │
    ├── Models/                       # Domain models (Card, Category, User, Feedback, etc.)
    ├── Views/                        # Razor views for Home and Admin modules
    └── wwwroot/                      # Static assets (images, CSS, JS, videos)
```

---

## Running Locally

### Prerequisites
- .NET 8.0 SDK installed on your system.

### Steps

1. Clone the repository:
   ```bash
   git clone https://github.com/AbdulWasayAli/Greetings-Project.git
   cd Greetings-Project/E-Project
   ```

2. Restore and run:
   ```bash
   dotnet restore
   dotnet run
   ```

3. Open your browser and visit:
   ```text
   http://localhost:5118
   ```

---

## Deployment

A `Dockerfile` is included in the project root for containerized deployment to platforms such as Render, Railway, or Azure Web Apps.
