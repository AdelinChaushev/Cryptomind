using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cryptomind.Data.Migrations
{
    /// <inheritdoc />
    public partial class fixed_user_seeding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
                columns: new[] { "ConcurrencyStamp", "NormalizedUserName", "PasswordHash" },
                values: new object[] { "d8bc6e7a-b8f8-4bac-8287-36c8242084e6", "ADMIN", "AQAAAAIAAYagAAAAEHBxvjlYt3zocyw1KEXenTNlD26PG53wqarvxeiCLB2fuZB+KSyQ8LfHJkEGRvbj1g==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b2c3d4e5-f6a7-8901-bcde-f12345678901",
                columns: new[] { "ConcurrencyStamp", "NormalizedUserName", "PasswordHash" },
                values: new object[] { "907d3d22-4ef4-4f47-be50-09b6df8a9c7f", "USER", "AQAAAAIAAYagAAAAEACCi+80Q2sAJioI9RZPI4liUBhGtt4qnFxH/4upis+DpIE/YfEviIi5+/34kbTJRQ==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
                columns: new[] { "ConcurrencyStamp", "NormalizedUserName", "PasswordHash" },
                values: new object[] { "a2e76b2e-c304-4ba4-867a-13f0b4a25c76", "ADMIN@CRYPTOMIND.COM", "AQAAAAIAAYagAAAAEPchmZoLAMWUfxz4NNIiQbFGoXXI75DCiIMtFdy2h3XaI2WiN/wjOEfd4G8vXZQIxg==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b2c3d4e5-f6a7-8901-bcde-f12345678901",
                columns: new[] { "ConcurrencyStamp", "NormalizedUserName", "PasswordHash" },
                values: new object[] { "be55590e-35b6-4d04-8883-25dfe4082a92", "USER@CRYPTOMIND.COM", "AQAAAAIAAYagAAAAEEOqmYcw3jMJgfpG37TfT8QEU/w0PHo8ar4YhyGDCYXrFGUujFHcYGs2GQhGayOHpA==" });
        }
    }
}
