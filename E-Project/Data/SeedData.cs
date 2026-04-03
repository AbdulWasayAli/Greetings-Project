using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using E_Project.Data;   // DbContext ke liye
using E_Project.Models; // Category class ke liye

public static class SeedData
{
    public static void Initialize(IServiceProvider serviceProvider)
    {
        using (var context = new ApplicationDbContext(
            serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
        {
            if (!context.Categories.Any())
            {
                context.Categories.AddRange(
                    new Category { CategoryName = "Birthday", ImagePath = "/User/images/1c.jpg" },
                    new Category { CategoryName = "Wedding", ImagePath = "/User/images/2c.jpg" },
                    new Category { CategoryName = "New Year", ImagePath = "/User/images/3c.jpg" },
                    new Category { CategoryName = "Eid-ul-Fitr", ImagePath = "/User/images/4c.jpg" },
                    new Category { CategoryName = "Eid-ul-Adha", ImagePath = "/User/images/5c.jpg" },
                    new Category { CategoryName = "Christmas", ImagePath = "/User/images/6c.jpg" },
                    new Category { CategoryName = "Thanksgiving", ImagePath = "/User/images/7c.webp" },
                    new Category { CategoryName = "Diwali", ImagePath = "/User/images/8c.jpg" }
                );
                context.SaveChanges();
            }

            // Seed Cards with different MediaPath values for each category
            if (!context.Cards.Any())
            {
                context.Cards.AddRange(
                    // Birthday Cards
                    new Card { CategoryID = 1, Title = "Wishing You a Very Happy Birthday🎉", Message = "May your special day be as wonderful as you are!", MediaPath = "/User/images/1c.jpg" },
                    
                    // Wedding Cards  
                    new Card { CategoryID = 2, Title = "Congratulations on Your Wedding💒", Message = "Wishing you a lifetime of love and happiness together!", MediaPath = "/User/images/2c.jpg" },
                    
                    // New Year Cards
                    new Card { CategoryID = 3, Title = "Happy New Year🎊", Message = "May this new year bring you joy, success, and wonderful adventures!", MediaPath = "/User/images/3c.jpg" },
                    
                    // Eid-ul-Fitr Cards
                    new Card { CategoryID = 4, Title = "Eid Mubarak🌙", Message = "May this blessed occasion bring peace, happiness and prosperity to you and your loved ones!", MediaPath = "/User/images/4c.jpg" },
                    
                    // Eid-ul-Adha Cards
                    new Card { CategoryID = 5, Title = "Blessings of Eid to You & Your Family🕌", Message = "Wishing you and your family health, wealth, and prosperity this Eid.", MediaPath = "/User/images/5c.jpg" },
                    
                    // Christmas Cards
                    new Card { CategoryID = 6, Title = "Merry Christmas🎄", Message = "May your Christmas be filled with love, laughter, and wonderful memories!", MediaPath = "/User/images/6c.jpg" },
                    
                    // Thanksgiving Cards  
                    new Card { CategoryID = 7, Title = "Happy Thanksgiving🦃", Message = "Grateful for all the wonderful moments we've shared. Happy Thanksgiving!", MediaPath = "/User/images/7c.webp" },
                    
                    // Diwali Cards
                    new Card { CategoryID = 8, Title = "Happy Diwali🪔", Message = "May the festival of lights illuminate your path with joy, prosperity, and happiness!", MediaPath = "/User/images/8c.jpg" }
                );
                context.SaveChanges();
            }
        }
    }
}
