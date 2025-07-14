using Microsoft.EntityFrameworkCore;
using QuizMaster.Models;

namespace QuizMaster.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.Username).IsUnique();

                entity.Property(e => e.FirstName).HasMaxLength(50);
                entity.Property(e => e.LastName).HasMaxLength(50);
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.Username).HasMaxLength(50);
                entity.Property(e => e.OrganizationName).HasMaxLength(100);
            });

            modelBuilder.Entity<Quiz>(entity =>
            {
                entity.Property(e => e.Name).HasMaxLength(200);
                entity.Property(e => e.LocationName).HasMaxLength(100);
                entity.Property(e => e.Address).HasMaxLength(200);
                entity.Property(e => e.EntryFee).HasColumnType("decimal(10,2)");

                entity.HasOne(q => q.Category).WithMany(c => c.Quizzes).HasForeignKey(q => q.CategoryId).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Team>(entity =>
            {
                entity.Property(e => e.Name).HasMaxLength(100);
            });

            modelBuilder.Entity<Subscription>(entity =>
            {
                entity.HasIndex(e => new { e.SubscriberId, e.OrganizerId }).IsUnique();

                entity.HasOne(s => s.Subscriber)
                    .WithMany(u => u.Subscriptions)
                    .HasForeignKey(s => s.SubscriberId)
                    .OnDelete(DeleteBehavior.Cascade); 

                entity.HasOne(s => s.Organizer)
                    .WithMany()
                    .HasForeignKey(s => s.OrganizerId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "ADMIN" },
                new Role { Id = 2, Name = "ORGANIZER" },
                new Role { Id = 3, Name = "COMPETITOR" }
            );
        }
    }
}
