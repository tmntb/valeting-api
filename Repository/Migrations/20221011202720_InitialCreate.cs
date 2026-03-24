using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repository.Migrations;

[ExcludeFromCodeCoverage]
public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
           name: "RD_Status",
           columns: table => new
           {
               Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
               Code = table.Column<string>(type: "nvarchar(50)", nullable: false),
               Name = table.Column<string>(type: "nvarchar(100)", nullable: false),
               Active = table.Column<bool>(type: "bit", nullable: false)
           },
           constraints: table =>
           {
               table.PrimaryKey("PK_RD_Status", x => x.Id);
           });

        migrationBuilder.CreateTable(
           name: "RD_Role",
           columns: table => new
           {
               Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
               Code = table.Column<string>(type: "nvarchar(50)", nullable: false),
               Name = table.Column<string>(type: "nvarchar(100)", nullable: false),
               Active = table.Column<bool>(type: "bit", nullable: false)
           },
           constraints: table =>
           {
               table.PrimaryKey("PK_RD_Role", x => x.Id);
           });

        migrationBuilder.CreateTable(
            name: "RD_Flexibility",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Code = table.Column<string>(type: "nvarchar(50)", nullable: false),
                Name = table.Column<string>(type: "nvarchar(100)", nullable: false),
                NumberOfMinutes = table.Column<int>(type: "int", nullable: false),
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
                Code = table.Column<string>(type: "nvarchar(50)", nullable: false),
                Name = table.Column<string>(type: "nvarchar(100)", nullable: false),
                Active = table.Column<bool>(type: "bit", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_RD_VehicleSize", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "ApplicationUser",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                FirstName = table.Column<string>(type: "nvarchar(50)", nullable: false),
                LastName = table.Column<string>(type: "nvarchar(50)", nullable: false),
                DateOfBirth = table.Column<DateOnly>(type: "date", nullable: false),
                ContactNumber = table.Column<int>(type: "int", nullable: false),
                Role_Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                IsActive = table.Column<bool>(type: "bit", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                LastLoginAt = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ApplicationUser", x => x.Id);
                table.ForeignKey(
                    name: "FK_ApplicationUser_Role",
                    column: x => x.Role_Id,
                    principalTable: "RD_Role",
                    principalColumn: "Id");
            });

        migrationBuilder.CreateTable(
            name: "Booking",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Reference = table.Column<string>(type: "nvarchar(50)", nullable: false),
                Customer_Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Flexibility_Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                VehicleSize_Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ScheduledAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                Status_Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                DecisionAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                DecisionBy_Id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                RequiresApproval = table.Column<bool>(type: "bit", nullable: false),
                Notes = table.Column<string>(type: "nvarchar(500)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Booking", x => x.Id);
                table.ForeignKey(
                    name: "FK_Booking_ApplicationUser_Customer",
                    column: x => x.Customer_Id,
                    principalTable: "ApplicationUser",
                    principalColumn: "Id");
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
                table.ForeignKey(
                    name: "FK_Booking_Status",
                    column: x => x.Status_Id,
                    principalTable: "RD_Status",
                    principalColumn: "Id");
                table.ForeignKey(
                    name: "FK_Booking_ApplicationUser_DecisionBy",
                    column: x => x.DecisionBy_Id,
                    principalTable: "ApplicationUser",
                    principalColumn: "Id");
            });

        migrationBuilder.CreateIndex(
            name: "IX_ApplicationUser_Role_Id",
            table: "ApplicationUser",
            column: "Role_Id");

        migrationBuilder.CreateIndex(
            name: "IX_Booking_Customer_Id",
            table: "Booking",
            column: "Customer_Id");

        migrationBuilder.CreateIndex(
            name: "IX_Booking_Flexibility_Id",
            table: "Booking",
            column: "Flexibility_Id");

        migrationBuilder.CreateIndex(
            name: "IX_Booking_VehicleSize_Id",
            table: "Booking",
            column: "VehicleSize_Id");

        migrationBuilder.CreateIndex(
            name: "IX_Booking_Status_Id",
            table: "Booking",
            column: "Status_Id");

        migrationBuilder.CreateIndex(
            name: "IX_Booking_DecisionBy_Id",
            table: "Booking",
            column: "DecisionBy_Id");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "ApplicationUser");

        migrationBuilder.DropTable(name: "Booking");

        migrationBuilder.DropTable(name: "RD_Flexibility");

        migrationBuilder.DropTable(name: "RD_VehicleSize");

        migrationBuilder.DropTable(name: "RD_Role");

        migrationBuilder.DropTable(name: "RD_Status");
    }
}
