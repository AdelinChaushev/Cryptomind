using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cryptomind.Data.Migrations
{
    /// <inheritdoc />
    public partial class RoomsWon_ApplicationUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RoomsWon",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "RoomsWon" },
                values: new object[] { "1c77bf7b-5080-4250-af44-1a24fee62f72", "AQAAAAIAAYagAAAAEKIyyYnqsa/wtdUkIN9SaiKBMxtaGI+MMbCy3pl/2BZrtOdM221ThCHpbesr2RlsWw==", 0 });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b2c3d4e5-f6a7-8901-bcde-f12345678901",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "RoomsWon" },
                values: new object[] { "535594f3-f335-4f63-80ea-58575d7e0c43", "AQAAAAIAAYagAAAAEH3iz2Y2bjfCI6P8PHqwJxCp1hxMUHmeDiiFW3i5qP9JqJn9hfsJPafZ+eWEBdZR6Q==", 0 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RoomsWon",
                table: "AspNetUsers");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "f4af1202-bb9e-46fb-8a31-57247038748d", "AQAAAAIAAYagAAAAEEaGeD2lolU5SnG3G5JD3vYTD1MwNEKsFHtWfnc9hEoKVxSxggFWToM3JnRPIrQkJQ==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b2c3d4e5-f6a7-8901-bcde-f12345678901",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "6b5d4063-62b0-427c-9ea2-b8971a17f9bc", "AQAAAAIAAYagAAAAEGTatZbTKk+VyoPJ2nS1aRDJ6Zrt0JDHg7EOBCypjIeiEqWeMSTiC74BirViNGpVEg==" });
        }
    }
}
