using Microsoft.EntityFrameworkCore;
using Western_Mutual_Api.Models;

namespace Western_Mutual_Api.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Buyer> Buyers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>()
                .HasOne<Buyer>() 
                .WithMany() 
                .HasForeignKey(p => p.BuyerId) 
                .OnDelete(DeleteBehavior.Cascade);
        }

    }
}
