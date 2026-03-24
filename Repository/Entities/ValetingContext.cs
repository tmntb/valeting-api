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
    public virtual DbSet<RdStatus> RdStatus { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseLazyLoadingProxies();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.ToTable("ApplicationUser");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("Id");

            entity.Property(e => e.Email).IsRequired();

            entity.Property(e => e.PasswordHash).IsRequired();

            entity.Property(e => e.FirstName)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.LastName)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.DateOfBirth)
                .IsRequired()
                .HasColumnType("date");

            entity.Property(e => e.ContactNumber).IsRequired();

            entity.Property(e => e.RoleId)
                .IsRequired()
                .HasColumnName("Role_Id");

            entity.Property(e => e.IsActive).IsRequired();

            entity.Property(e => e.CreatedAt)
                .IsRequired()
                .HasColumnType("datetime2");

            entity.Property(e => e.UpdatedAt).HasColumnType("datetime2");

            entity.Property(e => e.LastLoginAt).HasColumnType("datetime2");

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

            entity.Property(e => e.Reference)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.CustomerId)
                .IsRequired()
                .HasColumnName("Customer_Id");

            entity.Property(e => e.FlexibilityId)
                .IsRequired()
                .HasColumnName("Flexibility_Id");

            entity.Property(e => e.VehicleSizeId)
                .IsRequired()
                .HasColumnName("VehicleSize_Id");

            entity.Property(e => e.ScheduledAt)
                .IsRequired()
                .HasColumnType("datetime2");

            entity.Property(e => e.StatusId)
                .IsRequired()
                .HasColumnName("Status_Id");

            entity.Property(e => e.CreatedAt)
                .IsRequired()
                .HasColumnType("datetime2");

            entity.Property(e => e.UpdatedAt).HasColumnType("datetime2");

            entity.Property(e => e.DecisionAt).HasColumnType("datetime2");

            entity.Property(e => e.DecisionById).HasColumnName("DecisionBy_Id");

            entity.Property(e => e.RequiresApproval).IsRequired();

            entity.Property(e => e.Notes).HasMaxLength(500);

            entity.HasOne(d => d.Customer)
                .WithMany(p => p.CustomerBookings)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Booking_ApplicationUser");

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

            entity.HasOne(d => d.Status)
                .WithMany(p => p.Bookings)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Booking_Status");

            entity.HasOne(d => d.DecisionBy)
                .WithMany(p => p.DecisionBookings)
                .HasForeignKey(d => d.DecisionById)
                .HasConstraintName("FK_Booking_DecisionByApplicationUser");
        });

        modelBuilder.Entity<RdFlexibility>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.ToTable("RD_Flexibility");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("Id");

            entity.Property(e => e.Code)
                .HasConversion<string>()
                .HasMaxLength(50)
                .HasColumnName("Code");

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.NumberOfMinutes)
                .IsRequired();

            entity.Property(e => e.Active).IsRequired();
        });

        modelBuilder.Entity<RdVehicleSize>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.ToTable("RD_VehicleSize");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("Id");

            entity.Property(e => e.Code)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50);

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.Active).IsRequired();
        });

        modelBuilder.Entity<RdRole>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.ToTable("RD_Role");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("Id");

            entity.Property(e => e.Code)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50);

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.Active).IsRequired();
        });

        modelBuilder.Entity<RdStatus>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.ToTable("RD_Status");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("Id");

            entity.Property(e => e.Code)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50);

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.Active).IsRequired();
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}