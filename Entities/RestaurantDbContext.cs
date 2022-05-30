using Microsoft.EntityFrameworkCore;

namespace RestaurantApi.Entities;

public class RestaurantDbContext : DbContext
{
    private string _connectionString = "Server=localhost\\SQLEXPRESS;Database=RestaurantDb;Trusted_Connection=True;";
    public DbSet<Restaurant> Restaurants { get; set; }
    public DbSet<Address> Addresses { get; set; }
    public DbSet<Dish> Dishes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Dish>().Property(d => d.Name).IsRequired();
        modelBuilder.Entity<Dish>().Property(d => d.Price).HasColumnType("real");
        modelBuilder.Entity<Restaurant>().Property(r => r.Name).IsRequired().HasMaxLength(25);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(_connectionString);
    }
}