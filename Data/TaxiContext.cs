using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using TaxiDispatcher.Models;

namespace TaxiDispatcher.Data;

public partial class TaxiContext : DbContext
{
    public TaxiContext(DbContextOptions<TaxiContext> options) : base(options) { }
    public virtual DbSet<Role> Roles { get; set; }
    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<Order> Orders { get; set; }
    public virtual DbSet<OrderStatus> OrderStatuses { get; set; }
    public virtual DbSet<Tariff> Tariffs { get; set; }
    public virtual DbSet<CarClass> CarClasses { get; set; }
    public virtual DbSet<Invoice> Invoices { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Role>(entity => 
        {
            entity.HasKey(e => e.RoleId);
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.RoleName).HasMaxLength(50).HasColumnName("role_name");
        });

        modelBuilder.Entity<User>(entity => 
        {
            entity.HasKey(e => e.UserId);
            entity.HasIndex(e => e.Login).IsUnique();
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Login).HasColumnName("login");
            entity.Property(e => e.LastName).HasColumnName("last_name").HasMaxLength(60);
            entity.Property(e => e.FirstName).HasColumnName("first_name").HasMaxLength(60);
            entity.Property(e => e.MiddleName).HasColumnName("middle_name").HasMaxLength(60);
            entity.Property(e => e.Phone).HasColumnName("phone");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.PasswordHash).HasColumnName("password_hash");
            
            entity.HasOne(d => d.Role).WithMany(p => p.Users).HasForeignKey(d => d.RoleId);
        });

        modelBuilder.Entity<OrderStatus>(entity => 
        {
            entity.HasKey(e => e.StatusId);
            entity.Property(e => e.StatusId).HasColumnName("status_id");
            entity.Property(e => e.StatusName).HasMaxLength(50).HasColumnName("status_name");
        });

        modelBuilder.Entity<CarClass>(entity => 
        {
            entity.HasKey(e => e.ClassId);
            entity.Property(e => e.ClassId).HasColumnName("class_id");
            entity.Property(e => e.ClassName).HasMaxLength(50).HasColumnName("class_name");
        });

        modelBuilder.Entity<Tariff>(entity => 
        {
            entity.HasKey(e => e.TariffId);
            entity.Property(e => e.TariffId).HasColumnName("tariff_id");
            entity.Property(e => e.TariffName).HasMaxLength(50).HasColumnName("tariff_name");
            entity.Property(e => e.BaseFare).HasColumnName("base_fare").HasColumnType("decimal(8,2)");
            entity.Property(e => e.PricePerKm).HasColumnName("price_per_km").HasColumnType("decimal(8,2)");
            entity.Property(e => e.MinFare).HasColumnName("min_fare").HasColumnType("decimal(8,2)");
            entity.Property(e => e.CarClassId).HasColumnName("car_class_id");

            entity.HasOne(d => d.CarClass).WithMany(p => p.Tariffs).HasForeignKey(d => d.CarClassId);
        });

        modelBuilder.Entity<Order>(entity => 
        {
            entity.HasKey(e => e.OrderId);
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.OrderDate).HasColumnName("order_date");
            entity.Property(e => e.ClientName).HasColumnName("client_name");
            entity.Property(e => e.ClientPhone).HasColumnName("client_phone");
            entity.Property(e => e.PickupAddress).HasColumnName("pickup_address");
            entity.Property(e => e.DestinationAddress).HasColumnName("destination_address");
            entity.Property(e => e.DistanceKm).HasColumnName("distance_km").HasColumnType("decimal(6, 2)");
            entity.Property(e => e.StatusId).HasColumnName("status_id");
            entity.Property(e => e.TariffId).HasColumnName("tariff_id");
            
            entity.HasOne(d => d.Status).WithMany(p => p.Orders).HasForeignKey(d => d.StatusId);
            entity.HasOne(d => d.Tariff).WithMany().HasForeignKey(d => d.TariffId);
        });

        modelBuilder.Entity<Invoice>(entity => 
        {
            entity.HasKey(e => e.InvoiceId);
            entity.Property(e => e.InvoiceId).HasColumnName("invoice_id");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.Amount).HasColumnName("amount").HasColumnType("decimal(10,2)");
            entity.Property(e => e.PaymentStatus).HasColumnName("payment_status");
            
            entity.HasOne(d => d.Order).WithOne(p => p.Invoice).HasForeignKey<Invoice>(d => d.OrderId);
        });

        OnModelCreatingPartial(modelBuilder);
    }
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
