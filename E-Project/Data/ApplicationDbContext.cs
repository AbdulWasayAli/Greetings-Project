using E_Project.Models;
using Microsoft.EntityFrameworkCore;

namespace E_Project.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ContactMessage> ContactMessages { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Card> Cards { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<CardSend> CardSends { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Subscription relationships
            modelBuilder.Entity<Subscription>()
                .HasOne(s => s.User)
                .WithMany(u => u.Subscriptions)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure CardSend relationships
            modelBuilder.Entity<CardSend>()
                .HasOne(cs => cs.User)
                .WithMany(u => u.CardSends)
                .HasForeignKey(cs => cs.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CardSend>()
                .HasOne(cs => cs.Card)
                .WithMany()
                .HasForeignKey(cs => cs.CardId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure decimal precision for Amount
            modelBuilder.Entity<Subscription>()
                .Property(s => s.Amount)
                .HasColumnType("decimal(18,2)");
        }
    }
}
