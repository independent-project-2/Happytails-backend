using Microsoft.EntityFrameworkCore;
using HappyTailBackend.Models;

namespace HappyTailBackend.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        // DB sets
        public DbSet<User> Users { get; set; }
        public DbSet<Pet> Pets { get; set; }
        public DbSet<Blog> Blogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User-Pet relationship
            modelBuilder.Entity<User>()
                        .HasMany(u => u.Pets)
                        .WithOne(p => p.Owner)
                        .HasForeignKey(p => p.OwnerId)
                        .OnDelete(DeleteBehavior.Cascade);

            // Optional: additional constraints
            modelBuilder.Entity<User>()
                        .HasIndex(u => u.Email)
                        .IsUnique();
        }
    }
}
