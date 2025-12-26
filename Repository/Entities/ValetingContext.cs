using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace Repository.Entities;

[ExcludeFromCodeCoverage]
public partial class ValetingContext : DbContext
{
    public ValetingContext()
    {
    }

    public ValetingContext(DbContextOptions<ValetingContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ApplicationUser> ApplicationUsers { get; set; } = null!;
    public virtual DbSet<Booking> Bookings { get; set; } = null!;
    public virtual DbSet<RdFlexibility> RdFlexibilities { get; set; } = null!;
    public virtual DbSet<RdVehicleSize> RdVehicleSizes { get; set; } = null!;
    public virtual DbSet<RdRole> RdRoles { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseLazyLoadingProxies();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            entity.HasKey(e => e.Username);

            entity.ToTable("ApplicationUser");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("Id");

            entity.Property(e => e.Username).HasMaxLength(50);

            entity.Property(e => e.Password).IsRequired();

            entity.Property(e => e.RoleId).HasColumnName("Role_Id");

            entity.Property(e => e.IsActive);

            entity.HasOne(d => d.Role)
                .WithMany(p => p.ApplicationUsers)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ApplicationUser_Role");
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.ToTable("Booking");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("Id");

            entity.Property(e => e.BookingDate).HasColumnType("datetime");

            entity.Property(e => e.FlexibilityId).HasColumnName("Flexibility_Id");

            entity.Property(e => e.VehicleSizeId).HasColumnName("VehicleSize_Id");

            entity.HasOne(d => d.Flexibility)
                .WithMany(p => p.Bookings)
                .HasForeignKey(d => d.FlexibilityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Booking_Flexibility");

            entity.HasOne(d => d.VehicleSize)
                .WithMany(p => p.Bookings)
                .HasForeignKey(d => d.VehicleSizeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Booking_VehicleSize");
        });

        modelBuilder.Entity<RdFlexibility>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.ToTable("RD_Flexibility");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("Id");
        });

        modelBuilder.Entity<RdVehicleSize>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.ToTable("RD_VehicleSize");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("Id");
        });

        modelBuilder.Entity<RdRole>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.ToTable("RD_Role");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("Id");

            entity.Property(e => e.Name)
                .HasConversion<string>()
                .HasColumnName("Name");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}