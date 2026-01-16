using CarService.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CarService.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Vehicle> Vehicles { get; set; }

    public DbSet<Service> Services { get; set; }

    public DbSet<Part> Parts { get; set; }

    public DbSet<ServiceOrder> ServiceOrders { get; set; }

    public DbSet<ServiceOrderItem> ServiceOrderItems { get; set; }

    public DbSet<Review> Reviews { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ApplicationUser>()
            .HasMany(user => user.Vehicles)
            .WithOne(vehicle => vehicle.Owner)
            .HasForeignKey(vehicle => vehicle.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ApplicationUser>()
            .HasMany(user => user.ClientOrders)
            .WithOne(order => order.Client)
            .HasForeignKey(order => order.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ApplicationUser>()
            .HasMany(user => user.MechanicOrders)
            .WithOne(order => order.Mechanic)
            .HasForeignKey(order => order.MechanicId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Vehicle>()
            .HasMany(vehicle => vehicle.ServiceOrders)
            .WithOne(order => order.Vehicle)
            .HasForeignKey(order => order.VehicleId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ServiceOrder>()
            .HasMany(order => order.Items)
            .WithOne(item => item.ServiceOrder)
            .HasForeignKey(item => item.ServiceOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Service>()
            .HasMany(service => service.ServiceOrderItems)
            .WithOne(item => item.Service)
            .HasForeignKey(item => item.ServiceId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Part>()
            .HasMany(part => part.ServiceOrderItems)
            .WithOne(item => item.Part)
            .HasForeignKey(item => item.PartId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<ServiceOrder>()
            .HasOne(order => order.Review)
            .WithOne(review => review.ServiceOrder)
            .HasForeignKey<Review>(review => review.ServiceOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Vehicle>()
            .HasIndex(vehicle => vehicle.VIN)
            .IsUnique();

        modelBuilder.Entity<Vehicle>()
            .HasIndex(vehicle => vehicle.OwnerId);

        modelBuilder.Entity<ServiceOrder>()
            .HasIndex(order => order.ClientId);

        modelBuilder.Entity<ServiceOrder>()
            .HasIndex(order => order.MechanicId);

        modelBuilder.Entity<ServiceOrder>()
            .HasIndex(order => order.Status);

        modelBuilder.Entity<Service>()
            .Property(service => service.Price)
            .HasPrecision(18, 2);
    }
}
