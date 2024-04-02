using Microsoft.EntityFrameworkCore;
using SharedModels;

namespace DataAccessLayer
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Customer> Customers { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed the Customers table with a default entry
            modelBuilder.Entity<Customer>().HasData(
                new Customer
                {
                    Id = 1,
                    FirstName = "G",
                    LastName="K",
                    Email="gk@gmail.com",
                    LoginUser="gk@gmail.com",
                    Password= "fAG4bYyROtGJhlRd8PVc0A==",
                    PhoneNumber="123456789"
                }
            );
        }

    }
}