using Microsoft.EntityFrameworkCore;

namespace RestaurantApi.Entities;

public class RestaurantDbContext : DbContext
{
    public DbSet<Restaurant> Restaurants { get; set; } = null!;
    public DbSet<Address> Addresses { get; set; } = null!;
    public DbSet<Dish> Dishes { get; set; } = null!;
    public DbSet<Role> Roles { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;

    public RestaurantDbContext(DbContextOptions<RestaurantDbContext> options) : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Dish>().Property(d => d.Name).IsRequired();
        modelBuilder.Entity<Dish>().Property(d => d.Price).HasColumnType("real");
        modelBuilder.Entity<Restaurant>().Property(r => r.Name).IsRequired().HasMaxLength(25);

        modelBuilder.Entity<Address>().Property(a => a.Street).IsRequired().HasMaxLength(50);
        modelBuilder.Entity<Address>().Property(a => a.City).IsRequired().HasMaxLength(50);

        modelBuilder.Entity<User>().Property(u => u.Email).IsRequired();
        modelBuilder.Entity<Role>().Property(r => r.Name).IsRequired();
    }
}