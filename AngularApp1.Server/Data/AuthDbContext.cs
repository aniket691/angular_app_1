using Microsoft.EntityFrameworkCore;
using MoniteringSystem.Models;

namespace MoniteringSystem.Data
{
    public class AuthDbContext : DbContext
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) { }

        // DbSet property with a name that will be pluralized by convention
        public DbSet<User> User { get; set; }

        // Configure table name and schema if necessary
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure table name if it does not follow conventions
            modelBuilder.Entity<User>().ToTable("user");

            // Additional configurations can go here
        }
    }
}
