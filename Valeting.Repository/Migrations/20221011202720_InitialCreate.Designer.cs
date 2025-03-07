﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Valeting.Repository.Entities;

#nullable disable

namespace Valeting.Repository.Migrations
{
    [DbContext(typeof(ValetingContext))]
    [Migration("20221011202720_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("Valeting.Repository.Models.User.Entities.ApplicationUser", b =>
                {
                    b.Property<string>("Username")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("Id");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Salt")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Username");

                    b.ToTable("ApplicationUser", (string)null);
                });

            modelBuilder.Entity("Valeting.Repository.Models.User.Entities.Booking", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("Id");

                    b.Property<bool?>("Approved")
                        .HasColumnType("bit");

                    b.Property<DateTime>("BookingDate")
                        .HasColumnType("datetime");

                    b.Property<int>("ContactNumber")
                        .HasColumnType("int");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("FlexibilityId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("Flexibility_Id");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("VehicleSizeId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("VehicleSize_Id");

                    b.HasKey("Id");

                    b.HasIndex("FlexibilityId");

                    b.HasIndex("VehicleSizeId");

                    b.ToTable("Booking", (string)null);
                });

            modelBuilder.Entity("Valeting.Repository.Models.User.Entities.RdFlexibility", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("Id");

                    b.Property<bool>("Active")
                        .HasColumnType("bit");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("RD_Flexibility", (string)null);
                });

            modelBuilder.Entity("Valeting.Repository.Models.User.Entities.RdVehicleSize", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("Id");

                    b.Property<bool>("Active")
                        .HasColumnType("bit");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("RD_VehicleSize", (string)null);
                });

            modelBuilder.Entity("Valeting.Repository.Models.User.Entities.Booking", b =>
                {
                    b.HasOne("Valeting.Repository.Models.User.Entities.RdFlexibility", "Flexibility")
                        .WithMany("Bookings")
                        .HasForeignKey("FlexibilityId")
                        .IsRequired()
                        .HasConstraintName("FK_Booking_Flexibility");

                    b.HasOne("Valeting.Repository.Models.User.Entities.RdVehicleSize", "VehicleSize")
                        .WithMany("Bookings")
                        .HasForeignKey("VehicleSizeId")
                        .IsRequired()
                        .HasConstraintName("FK_Booking_VehicleSize");

                    b.Navigation("Flexibility");

                    b.Navigation("VehicleSize");
                });

            modelBuilder.Entity("Valeting.Repository.Models.User.Entities.RdFlexibility", b =>
                {
                    b.Navigation("Bookings");
                });

            modelBuilder.Entity("Valeting.Repository.Models.User.Entities.RdVehicleSize", b =>
                {
                    b.Navigation("Bookings");
                });
#pragma warning restore 612, 618
        }
    }
}
