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
                    new Category { CategoryID = 1, CategoryName = "Birthday", ImagePath = "/User/images/1c.jpg" },
                    new Category { CategoryID = 2, CategoryName = "Wedding", ImagePath = "/User/images/2c.jpg" },
                    new Category { CategoryID = 3, CategoryName = "New Year", ImagePath = "/User/images/3c.jpg" },
                    new Category { CategoryID = 4, CategoryName = "Eid-ul-Fitr", ImagePath = "/User/images/4c.jpg" },
                    new Category { CategoryID = 5, CategoryName = "Eid-ul-Adha", ImagePath = "/User/images/5c.jpg" },
                    new Category { CategoryID = 6, CategoryName = "Christmas", ImagePath = "/User/images/6c.jpg" },
                    new Category { CategoryID = 7, CategoryName = "Thanksgiving", ImagePath = "/User/images/7c.webp" },
                    new Category { CategoryID = 8, CategoryName = "Diwali", ImagePath = "/User/images/8c.jpg" }
                );
                context.SaveChanges();
            }

            // Cards seeding
            if (context.Cards.Count() < 20)
            {
                context.Cards.RemoveRange(context.Cards);
                context.SaveChanges();

                var cards = new List<Card>
                {
                    // 1. Birthday Cards (CategoryID = 1)
                    new Card { CategoryID = 1, Title = "Happy Birthday! 🎉", Message = "Wishing you a day filled with happiness and a year filled with joy.", MediaPath = "/User/images/1.jpg" },
                    new Card { CategoryID = 1, Title = "Have a Wonderful Birthday 🎂", Message = "Sending you smiles for every moment of your special day.", MediaPath = "/User/images/2.jpg" },
                    new Card { CategoryID = 1, Title = "Best Wishes on Your Birthday 🎈", Message = "Hope your special day brings you all that your heart desires!", MediaPath = "/User/images/3.avif" },
                    new Card { CategoryID = 1, Title = "Celebrate in Style 🥳", Message = "May your birthday be as incredible and wonderful as you are.", MediaPath = "/User/images/4.avif" },
                    new Card { CategoryID = 1, Title = "Warmest Birthday Wishes 🎁", Message = "Wishing you good health, love and prosperity on your special day.", MediaPath = "/User/images/5.avif" },
                    new Card { CategoryID = 1, Title = "Cheers to Another Year 🥂", Message = "May this year bring endless opportunities and joy.", MediaPath = "/User/images/6.avif" },
                    new Card { CategoryID = 1, Title = "Make a Wish! 🕯️", Message = "Blow the candles and make the sweetest wish ever.", MediaPath = "/User/images/7.avif" },
                    new Card { CategoryID = 1, Title = "Special Day, Special You ✨", Message = "Thinking of you on your birthday and wishing you everything happy.", MediaPath = "/User/images/8.avif" },
                    new Card { CategoryID = 1, Title = "Joyful Birthday Greetings 🎊", Message = "Wishing you a blessed and beautiful birthday.", MediaPath = "/User/images/9.avif" },
                    new Card { CategoryID = 1, Title = "Happy Birthday to a True Star ⭐", Message = "Keep shining bright today and throughout the upcoming year.", MediaPath = "/User/images/10.avif" },
                    new Card { CategoryID = 1, Title = "Fun, Laughter & Cakes 🍰", Message = "Eat cake, laugh loud, and enjoy your amazing birthday!", MediaPath = "/User/images/11.avif" },
                    new Card { CategoryID = 1, Title = "Blessed Birthday! 🌸", Message = "May god bless you with abundant love, health and happiness.", MediaPath = "/User/images/12.avif" },

                    // 2. Wedding Cards (CategoryID = 2)
                    new Card { CategoryID = 2, Title = "Happy Wedding Day 💒", Message = "Wishing you a lifetime of love and happiness together!", MediaPath = "/User/images/1w.avif" },
                    new Card { CategoryID = 2, Title = "Two Hearts, One Soul 💍", Message = "May your married life be filled with sweet moments and love.", MediaPath = "/User/images/2w.avif" },
                    new Card { CategoryID = 2, Title = "Congratulations on Your Wedding 🥂", Message = "Wishing you harmony, peace, and eternal togetherness.", MediaPath = "/User/images/3w.avif" },
                    new Card { CategoryID = 2, Title = "A Beautiful Journey Begins 🕊️", Message = "Warmest congratulations and love on your wedding day.", MediaPath = "/User/images/4w.avif" },
                    new Card { CategoryID = 2, Title = "Together Forever 💐", Message = "May your love grow deeper and stronger with each passing day.", MediaPath = "/User/images/5w.avif" },
                    new Card { CategoryID = 2, Title = "Wishing You Endless Love ❤️", Message = "Congratulations to the wonderful couple on this memorable day.", MediaPath = "/User/images/6w.avif" },
                    new Card { CategoryID = 2, Title = "Happy Married Life 🌹", Message = "May laughter and romance brighten every day of your marriage.", MediaPath = "/User/images/7w.avif" },
                    new Card { CategoryID = 2, Title = "Love, Trust & Harmony 💫", Message = "Best wishes for a prosperous and blessed married life.", MediaPath = "/User/images/8w.avif" },
                    new Card { CategoryID = 2, Title = "Pure Bliss & Joy ✨", Message = "Here's to a lifetime of love, laughter, and happily ever after.", MediaPath = "/User/images/9w.avif" },
                    new Card { CategoryID = 2, Title = "Heartfelt Wedding Wishes 💖", Message = "May the bond you share bring you lifelong peace and fulfillment.", MediaPath = "/User/images/10w.avif" },
                    new Card { CategoryID = 2, Title = "Celebrate Your Love 🥂", Message = "Cheers to your new life together! Wishing you all the very best.", MediaPath = "/User/images/11w.avif" },
                    new Card { CategoryID = 2, Title = "Congratulations to the Couple! 🎊", Message = "Wishing you a joyful wedding and an even better marriage.", MediaPath = "/User/images/12w.avif" },

                    // 3. New Year Cards (CategoryID = 3)
                    new Card { CategoryID = 3, Title = "Happy New Year! 🎊", Message = "May this new year bring you immense happiness, peace and success.", MediaPath = "/User/images/1ny.avif" },
                    new Card { CategoryID = 3, Title = "New Beginnings ✨", Message = "Wishing you 365 days of good fortune, love, and laughter.", MediaPath = "/User/images/2ny.avif" },
                    new Card { CategoryID = 3, Title = "Cheers to the New Year 🥂", Message = "Raise your glass to a fabulous year ahead full of prosperity.", MediaPath = "/User/images/3ny.avif" },
                    new Card { CategoryID = 3, Title = "Sparkle & Shine 🎆", Message = "May the coming year illuminate your path with joy and victory.", MediaPath = "/User/images/4ny.avif" },
                    new Card { CategoryID = 3, Title = "Fresh Opportunities 🌟", Message = "Embrace the fresh chapters waiting to be written this year.", MediaPath = "/User/images/5ny.avif" },
                    new Card { CategoryID = 3, Title = "Golden Moments Ahead 🏆", Message = "May all your dreams and resolutions turn into golden triumphs.", MediaPath = "/User/images/6ny.avif" },
                    new Card { CategoryID = 3, Title = "Count Your Blessings 🙏", Message = "Wishing you healthy, wealthy and joyful times throughout the year.", MediaPath = "/User/images/7ny.avif" },
                    new Card { CategoryID = 3, Title = "Joyful New Year Greetings 🎈", Message = "Have a fantastic and cheerful new year with family and friends.", MediaPath = "/User/images/8ny.avif" },
                    new Card { CategoryID = 3, Title = "Bright Future Awaits ☀️", Message = "May hope and determination lead you to greatness this year.", MediaPath = "/User/images/9ny.avif" },
                    new Card { CategoryID = 3, Title = "Celebration Time! 🥳", Message = "Celebrate the good times and welcome the new adventures ahead.", MediaPath = "/User/images/10ny.avif" },
                    new Card { CategoryID = 3, Title = "Peace & Harmony 🕊️", Message = "Sending warm prayers for world peace and personal prosperity.", MediaPath = "/User/images/11ny.avif" },
                    new Card { CategoryID = 3, Title = "Happy New Year to All! 🎉", Message = "May happiness never leave your home in this coming year.", MediaPath = "/User/images/12ny.avif" },

                    // 4. Eid-ul-Fitr Cards (CategoryID = 4)
                    new Card { CategoryID = 4, Title = "Eid Mubarak! 🌙", Message = "May this blessed day bring peace, happiness and prosperity to you and your family.", MediaPath = "/User/images/1ef.jpg" },
                    new Card { CategoryID = 4, Title = "Blessings of Eid 🕌", Message = "May Allah accept your good deeds and forgive all transgressions.", MediaPath = "/User/images/2ef.jpg" },
                    new Card { CategoryID = 4, Title = "Joyful Eid Greetings ✨", Message = "Wishing you a sweet and memorable Eid filled with warmth and love.", MediaPath = "/User/images/3ef.webp" },
                    new Card { CategoryID = 4, Title = "Eid Saeed! 🌸", Message = "May Allah shower his countless blessings on you on this sacred occasion.", MediaPath = "/User/images/4ef.jpg" },
                    new Card { CategoryID = 4, Title = "Celebration of Faith 🤲", Message = "May the divine grace of Allah illuminate your heart and home.", MediaPath = "/User/images/5ef.jpg" },
                    new Card { CategoryID = 4, Title = "Happy Eid-ul-Fitr 💫", Message = "May your life be as sweet as sewaiyan and bright as the crescent moon.", MediaPath = "/User/images/6ef.jpg" },
                    new Card { CategoryID = 4, Title = "Peace & Smiles on Eid 🕊️", Message = "Sending heartwarming wishes to you and your loved ones on Eid.", MediaPath = "/User/images/7ef.avif" },
                    new Card { CategoryID = 4, Title = "Eid Blessings to Family 👨‍👩‍👧‍👦", Message = "May this Eid bring eternal harmony and joy to your family.", MediaPath = "/User/images/8ef.jpg" },
                    new Card { CategoryID = 4, Title = "Spiritual Joy 🌙", Message = "Cherish the moments of forgiveness, gratitude, and devotion.", MediaPath = "/User/images/9ef.avif" },
                    new Card { CategoryID = 4, Title = "Greetings on Eid-ul-Fitr 🎉", Message = "Wishing you abundance and delightful smiles all day long.", MediaPath = "/User/images/10ef.jpg" },
                    new Card { CategoryID = 4, Title = "May Allah Bless You 🤲", Message = "Prayers for health, wealth, and spiritual growth on this Eid.", MediaPath = "/User/images/11ef.jpg" },
                    new Card { CategoryID = 4, Title = "Warm Eid Wishes 🌹", Message = "May the beauty of Eid remain in your heart throughout the year.", MediaPath = "/User/images/12ef.jpg" },

                    // 5. Eid-ul-Adha Cards (CategoryID = 5)
                    new Card { CategoryID = 5, Title = "Eid-ul-Adha Mubarak! 🕌", Message = "Blessings of Eid to you and your family on this holy festival of sacrifice.", MediaPath = "/User/images/1ea.jpg" },
                    new Card { CategoryID = 5, Title = "Spirit of Sacrifice 🐑", Message = "May Allah accept your sacrifices and answer your earnest prayers.", MediaPath = "/User/images/2ea.jpg" },
                    new Card { CategoryID = 5, Title = "Bakra Eid Mubarak 🌙", Message = "Wishing you joy, good feast and unity with your family and neighbors.", MediaPath = "/User/images/3ea.avif" },
                    new Card { CategoryID = 5, Title = "Divine Blessings ✨", Message = "May your devotion and sacrifices be honored with divine rewards.", MediaPath = "/User/images/4ea.jpg" },
                    new Card { CategoryID = 5, Title = "Happy Eid-ul-Adha 🌸", Message = "May your life be blessed with good health, faith, and unending prosperity.", MediaPath = "/User/images/5ea.webp" },
                    new Card { CategoryID = 5, Title = "Sacrifice & Gratitude 🤲", Message = "Remembering the true essence of sacrifice and compassion today.", MediaPath = "/User/images/6ea.jpg" },
                    new Card { CategoryID = 5, Title = "Peaceful Eid Greetings 🕊️", Message = "May Allah grant peace to your soul and your household this Eid.", MediaPath = "/User/images/7ea.webp" },
                    new Card { CategoryID = 5, Title = "Warmest Eid-ul-Adha Wishes 🌟", Message = "May this festival bring you closer to Allah and your dear ones.", MediaPath = "/User/images/8ea.jpg" },
                    new Card { CategoryID = 5, Title = "Joy of Giving 🎁", Message = "Spread love, share your blessings, and make someone smile today.", MediaPath = "/User/images/9ea.jpg" },
                    new Card { CategoryID = 5, Title = "Heartfelt Eid Greetings 💖", Message = "Wishing you a very blessed and rewarding Eid-ul-Adha.", MediaPath = "/User/images/10ea.webp" },
                    new Card { CategoryID = 5, Title = "Faith & Devotion 🌙", Message = "May the lessons of obedience and faith illuminate your path.", MediaPath = "/User/images/11ea.avif" },
                    new Card { CategoryID = 5, Title = "Eid Mubarak to You! 🎊", Message = "Wishing you tasty feasts and joyous gatherings this Bakra Eid.", MediaPath = "/User/images/12ea.jpg" },

                    // 6. Christmas Cards (CategoryID = 6)
                    new Card { CategoryID = 6, Title = "Merry Christmas! 🎄", Message = "May your Christmas be filled with love, laughter, and sweet memories!", MediaPath = "/User/images/1ch.jpg" },
                    new Card { CategoryID = 6, Title = "Joy to the World 🎅", Message = "Wishing you wonder, peace, and holiday cheer all season long.", MediaPath = "/User/images/2ch.jpg" },
                    new Card { CategoryID = 6, Title = "Season's Greetings ❄️", Message = "May the miracle of Christmas fill your heart with joy and warmth.", MediaPath = "/User/images/3ch.jpg" },
                    new Card { CategoryID = 6, Title = "Holiday Warmth 🔥", Message = "Sending you the warmest wishes for a wonderful festive holiday.", MediaPath = "/User/images/4ch.png" },
                    new Card { CategoryID = 6, Title = "Peace on Earth 🕊️", Message = "May peace and goodwill be with you and yours this Christmas.", MediaPath = "/User/images/5ch.jpg" },
                    new Card { CategoryID = 6, Title = "Festive Joy & Cheer 🎁", Message = "Hoping your holiday season is filled with delicious treats and gifts.", MediaPath = "/User/images/6ch.avif" },
                    new Card { CategoryID = 6, Title = "Christmas Blessings ✨", Message = "May faith, hope, and love guide you throughout the holidays.", MediaPath = "/User/images/7ch.webp" },
                    new Card { CategoryID = 6, Title = "Magical Christmas Moments ⭐", Message = "Cherish the moments of togetherness with your beloved family.", MediaPath = "/User/images/8ch.jpg" },
                    new Card { CategoryID = 6, Title = "Jingle All the Way 🔔", Message = "Wishing you laughter, joy, and wonderful surprises under the tree.", MediaPath = "/User/images/9ch.webp" },
                    new Card { CategoryID = 6, Title = "Merry & Bright 🕯️", Message = "May your holidays sparkle with moments of love and laughter.", MediaPath = "/User/images/10ch.webp" },
                    new Card { CategoryID = 6, Title = "Heartfelt Holiday Wishes ❤️", Message = "Thinking of you fondly during this most wonderful time of the year.", MediaPath = "/User/images/11ch.jpg" },
                    new Card { CategoryID = 6, Title = "Merry Christmas and Happy New Year 🎊", Message = "Best wishes for a delightful holiday and a prosperous year ahead.", MediaPath = "/User/images/12ch.avif" },

                    // 7. Thanksgiving Cards (CategoryID = 7)
                    new Card { CategoryID = 7, Title = "Happy Thanksgiving! 🦃", Message = "Grateful for all the wonderful moments and people in our lives.", MediaPath = "/User/images/1tg.avif" },
                    new Card { CategoryID = 7, Title = "Grateful Hearts 🍂", Message = "Wishing you a harvest of blessings, good health, and sweet memories.", MediaPath = "/User/images/2tg.avif" },
                    new Card { CategoryID = 7, Title = "Warm Thanksgiving Wishes 🥧", Message = "May your home be full of laughter, love, and plenty of delicious food.", MediaPath = "/User/images/3tg.avif" },
                    new Card { CategoryID = 7, Title = "Count Your Blessings 🍁", Message = "Gratitude turns what we have into enough. Have a blessed Thanksgiving.", MediaPath = "/User/images/4tg.avif" },
                    new Card { CategoryID = 7, Title = "Autumn Blessings 🌾", Message = "May the peace and beauty of the season surround you and your loved ones.", MediaPath = "/User/images/5tg.avif" },
                    new Card { CategoryID = 7, Title = "Feast & Family 🍽️", Message = "Wishing you a warm gathering of family, friendship, and joy.", MediaPath = "/User/images/6tg.avif" },
                    new Card { CategoryID = 7, Title = "Thankful for You 🧡", Message = "Sending gratitude and warm thoughts across the miles this holiday.", MediaPath = "/User/images/7tg.webp" },
                    new Card { CategoryID = 7, Title = "Joyful Thanksgiving Day 🌻", Message = "May your holiday be rich with harmony, gratitude, and good times.", MediaPath = "/User/images/8tg.jpg" },
                    new Card { CategoryID = 7, Title = "Harvest of Happiness 🌽", Message = "Hoping you have a bountiful season filled with joy and celebration.", MediaPath = "/User/images/9tg.png" },
                    new Card { CategoryID = 7, Title = "Blessings to Your Table 🕯️", Message = "May peace and love be guests at your table today and always.", MediaPath = "/User/images/10tg.webp" },
                    new Card { CategoryID = 7, Title = "Heartfelt Thanks 🙏", Message = "Wishing you warmth and happiness as we celebrate Thanksgiving.", MediaPath = "/User/images/11tg.jpg" },
                    new Card { CategoryID = 7, Title = "Happy Thanksgiving to All! 🍂", Message = "May your heart be filled with gratitude and contentment.", MediaPath = "/User/images/12tg.webp" },

                    // 8. Diwali Cards (CategoryID = 8)
                    new Card { CategoryID = 8, Title = "Happy Diwali! 🪔", Message = "May the festival of lights illuminate your path with joy, prosperity, and happiness!", MediaPath = "/User/images/1dw.avif" },
                    new Card { CategoryID = 8, Title = "Festival of Lights ✨", Message = "May light triumph over darkness and bring wisdom and peace into your home.", MediaPath = "/User/images/2dw.jpg" },
                    new Card { CategoryID = 8, Title = "Shubh Deepavali 🌟", Message = "Wishing you and your family immense success, health, and bright fortunes.", MediaPath = "/User/images/3dw.jpg" },
                    new Card { CategoryID = 8, Title = "Sparkles of Joy 🎆", Message = "May this Diwali burst with joyful moments and unforgettable celebrations.", MediaPath = "/User/images/4dw.jpg" },
                    new Card { CategoryID = 8, Title = "Prosperous Diwali Wishes 🪙", Message = "May Goddess Lakshmi bless you with abundance and prosperity.", MediaPath = "/User/images/5dw.jpg" },
                    new Card { CategoryID = 8, Title = "Diyas & Sweets 🍬", Message = "May your life be as sweet as treats and as bright as diyas.", MediaPath = "/User/images/6dw.jpg" },
                    new Card { CategoryID = 8, Title = "Joyful Diwali Greetings 🌸", Message = "Sending warmth, love, and light to your wonderful family this festive season.", MediaPath = "/User/images/7dw.png" },
                    new Card { CategoryID = 8, Title = "Celebration of Triumph 🏆", Message = "May your dreams light up like fireworks in the sky tonight.", MediaPath = "/User/images/8dw.png" },
                    new Card { CategoryID = 8, Title = "Peace & Happiness 🕊️", Message = "Wishing you a peaceful, joyful, and radiant Deepavali.", MediaPath = "/User/images/9dw.jpg" },
                    new Card { CategoryID = 8, Title = "Golden Blessings 💫", Message = "May good health, wealth, and prosperity shine upon your home.", MediaPath = "/User/images/10dw.jpg" },
                    new Card { CategoryID = 8, Title = "Festive Cheer & Laughter 🥳", Message = "Celebrate the beauty of togetherness this auspicious festival of lights.", MediaPath = "/User/images/11dw.webp" },
                    new Card { CategoryID = 8, Title = "Happy Diwali to Everyone! 🪔", Message = "May the light of Diwali guide you towards peace and boundless joy.", MediaPath = "/User/images/12dw.webp" }
                };

                context.Cards.AddRange(cards);
                context.SaveChanges();
            }

            // Admin User seeding
            if (!context.Users.Any(u => u.Role == "Admin"))
            {
                context.Users.Add(new User
                {
                    Name = "Admin",
                    Email = "admin@greetings.com",
                    Password = "admin",
                    Phone = "1234567890",
                    Role = "Admin"
                });
                context.SaveChanges();
            }
        }
    }
}
