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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__760965CC58D14F82");
            entity.HasIndex(e => e.RoleName, "UQ__Roles__783254B1E210467A").IsUnique();
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.RoleName).HasMaxLength(50).HasColumnName("role_name");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__B9BE370F638FE254");
            entity.HasIndex(e => e.Login, "UQ__Users__7838F2725324F593").IsUnique();
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.FullName).HasMaxLength(150).HasColumnName("full_name");
            entity.Property(e => e.Login).HasMaxLength(50).HasColumnName("login");
            entity.Property(e => e.Phone).HasMaxLength(20).HasColumnName("phone");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.PasswordHash).HasMaxLength(255).HasColumnName("password_hash");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<OrderStatus>(entity =>
        {
            entity.HasKey(e => e.StatusId);
            entity.Property(e => e.StatusId).HasColumnName("status_id");
            entity.Property(e => e.StatusName).HasMaxLength(50).HasColumnName("status_name").IsRequired();
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId);
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.OrderDate).HasColumnName("order_date");
            entity.Property(e => e.ClientName).HasMaxLength(150).HasColumnName("client_name").IsRequired();
            entity.Property(e => e.ClientPhone).HasMaxLength(20).HasColumnName("client_phone").IsRequired();
            entity.Property(e => e.PickupAddress).HasMaxLength(250).HasColumnName("pickup_address").IsRequired();
            entity.Property(e => e.DestinationAddress).HasMaxLength(250).HasColumnName("destination_address").IsRequired();
            entity.Property(e => e.DistanceKm).HasColumnType("decimal(6, 2)").HasColumnName("distance_km");
            entity.Property(e => e.StatusId).HasColumnName("status_id");

            entity.HasOne(d => d.Status).WithMany(p => p.Orders)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        OnModelCreatingPartial(modelBuilder);
    }
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
