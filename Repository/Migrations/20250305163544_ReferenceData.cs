using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repository.Migrations
{
    /// <inheritdoc />
    public partial class ReferenceData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "RD_VehicleSize",
                columns: ["Id", "Description", "Active"],
                values: new object[,]
                {
                    { "7536b1e6-a5eb-4156-931f-2562f4917000", "Small", true },
                    { "b1c0d62b-b422-4db2-a038-5d1048bd90c9", "Medium", true },
                    { "995cc987-395e-40d9-a8c3-e9070c8fd1c8", "Large", true },
                    { "e8220d6e-2aa6-4183-bd55-820e0e1635b3", "Van", true }
                }
            );

            migrationBuilder.InsertData(
                table: "RD_Flexibility",
                columns: ["Id", "Description", "Active"],
                values: new object[,]
                {
                    { "5afb5192-45f3-418d-8a48-bbecdeedd9e9", "+/- 1 Day", true },
                    { "e3cff703-3cd2-4253-999d-8230b8a550e2", "+/- 2 Days", true },
                    { "2209acff-4bad-4e6e-9262-5d7ede5bef81", "+/- 3 Days", true },
                }
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RD_VehicleSize",
                keyColumn: "Id",
                keyValues: ["7536b1e6-a5eb-4156-931f-2562f4917000", "b1c0d62b-b422-4db2-a038-5d1048bd90c9", "995cc987-395e-40d9-a8c3-e9070c8fd1c8", "e8220d6e-2aa6-4183-bd55-820e0e1635b3"]
            );

            migrationBuilder.DeleteData(
                table: "RD_Flexibility",
                keyColumn: "Id",
                keyValues: ["5afb5192-45f3-418d-8a48-bbecdeedd9e9", "e3cff703-3cd2-4253-999d-8230b8a550e2", "2209acff-4bad-4e6e-9262-5d7ede5bef81"]
            );
        }
    }
}
