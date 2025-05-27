using Microsoft.EntityFrameworkCore;
using SmartRide.API.Models;

namespace SmartRide.API.Data; 
public class SmartRideDbContext : DbContext
{
    public SmartRideDbContext(DbContextOptions<SmartRideDbContext> options) : base(options) { }

    public DbSet<Passenger> Passengers { get; set; }
    public DbSet<Driver> Drivers { get; set; }
    public DbSet<Ride> Rides { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Ride>(e =>
        {
            e.OwnsOne(r => r.PickupLocation);
            e.OwnsOne(r => r.DropoffLocation);
            e.Property(r => r.Fare).HasPrecision(18, 2);
        });
        modelBuilder.Entity<Driver>(e =>
        {
            e.OwnsOne(d => d.CurrentLocation);
        });
        modelBuilder.Entity<User>().ToTable("Users");
    }
}