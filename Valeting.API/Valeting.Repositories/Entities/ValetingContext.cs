using Microsoft.EntityFrameworkCore;

namespace Valeting.Repositories.Entities;

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

            entity.Property(e => e.Username).HasMaxLength(50);

            entity.Property(e => e.Id).HasColumnName("ID");

            entity.Property(e => e.Salt).HasMaxLength(50);
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.ToTable("Booking");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");

            entity.Property(e => e.BookingDate).HasColumnType("datetime");

            entity.Property(e => e.FlexibilityId).HasColumnName("Flexibility_ID");

            entity.Property(e => e.VehicleSizeId).HasColumnName("VehicleSize_ID");

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
            entity.ToTable("RD_Flexibility");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
        });

        modelBuilder.Entity<RdVehicleSize>(entity =>
        {
            entity.ToTable("RD_VehicleSize");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}