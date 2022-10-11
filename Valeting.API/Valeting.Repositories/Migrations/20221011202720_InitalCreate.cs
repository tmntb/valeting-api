using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Valeting.Repositories.Migrations
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
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RD_Flexibility", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "RD_VehicleSize",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RD_VehicleSize", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Booking",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BookingDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    Flexibility_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VehicleSize_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContactNumber = table.Column<int>(type: "int", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Approved = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Booking", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Booking_Flexibility",
                        column: x => x.Flexibility_ID,
                        principalTable: "RD_Flexibility",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Booking_VehicleSize",
                        column: x => x.VehicleSize_ID,
                        principalTable: "RD_VehicleSize",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Booking_Flexibility_ID",
                table: "Booking",
                column: "Flexibility_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Booking_VehicleSize_ID",
                table: "Booking",
                column: "VehicleSize_ID");
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
