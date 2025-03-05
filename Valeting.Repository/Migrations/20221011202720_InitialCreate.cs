﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Valeting.Repository.Migrations
{
    public partial class InitalCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApplicationUser",
                columns: table => new
                {
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Salt = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUser", x => x.Username);
                });

            migrationBuilder.CreateTable(
                name: "RD_Flexibility",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RD_Flexibility", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RD_VehicleSize",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RD_VehicleSize", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Booking",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BookingDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    Flexibility_Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VehicleSize_Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContactNumber = table.Column<int>(type: "int", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Approved = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {   
                    table.PrimaryKey("PK_Booking", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Booking_Flexibility",
                        column: x => x.Flexibility_Id,
                        principalTable: "RD_Flexibility",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Booking_VehicleSize",
                        column: x => x.VehicleSize_Id,
                        principalTable: "RD_VehicleSize",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Booking_Flexibility_Id",
                table: "Booking",
                column: "Flexibility_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Booking_VehicleSize_Id",
                table: "Booking",
                column: "VehicleSize_Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationUser");

            migrationBuilder.DropTable(
                name: "Booking");

            migrationBuilder.DropTable(
                name: "RD_Flexibility");

            migrationBuilder.DropTable(
                name: "RD_VehicleSize");
        }
    }
}
