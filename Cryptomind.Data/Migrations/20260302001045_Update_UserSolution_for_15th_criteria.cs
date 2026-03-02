using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cryptomind.Data.Migrations
{
    /// <inheritdoc />
    public partial class Update_UserSolution_for_15th_criteria : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsRareSolved",
                table: "UserSolution",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "1c4c787f-7864-4ea0-9b9f-a6a351da7cf0", "AQAAAAIAAYagAAAAECy+bBs48C27VzQa72zNi3E1NzzJ0jT6UPyF9BMqaPmTIz5GlQL39lFWLHitHW69Sg==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b2c3d4e5-f6a7-8901-bcde-f12345678901",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "ed2dc99d-f765-4907-b30b-74a7ee29f6fc", "AQAAAAIAAYagAAAAEPZVwCwgjVHmq+s6k6ej5gSxsnK64xaUHG9MtO6j6DPSm6htC4jSyrKarvxmmjgm7w==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRareSolved",
                table: "UserSolution");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "5044362d-ce67-44cd-bc40-1e6b30b9a0fd", "AQAAAAIAAYagAAAAEMZ+7v93fEvx+Yff+P0KZfqp+PKZwStJb0/r8yjwtKwAqvL7SCSkc/L6ihbmJLvvfQ==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b2c3d4e5-f6a7-8901-bcde-f12345678901",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "eb0b6baf-4f24-4f0e-8dd7-6071f689b432", "AQAAAAIAAYagAAAAEMez0YOFr7T2+WFsnrJTatPXMKswKlsUZcLdYGgnSZZf0iO/eH5gg+tYRVkJhDUWuw==" });
        }
    }
}
