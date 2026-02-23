using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cryptomind.Data.Migrations
{
    /// <inheritdoc />
    public partial class Seed_Badges_with_paths : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "Badges",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 1,
                column: "ImagePath",
                value: "../Images/Badges/Badge_1.png");

            migrationBuilder.UpdateData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 2,
                column: "ImagePath",
                value: "../Images/Badges/Badge_2.png");

            migrationBuilder.UpdateData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 3,
                column: "ImagePath",
                value: "../Images/Badges/Badge_3.png");

            migrationBuilder.UpdateData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 4,
                column: "ImagePath",
                value: "../Images/Badges/Badge_4.png");

            migrationBuilder.UpdateData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 5,
                column: "ImagePath",
                value: "../Images/Badges/Badge_5.png");

            migrationBuilder.UpdateData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 6,
                column: "ImagePath",
                value: "../Images/Badges/Badge_6.png");

            migrationBuilder.UpdateData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 7,
                column: "ImagePath",
                value: "../Images/Badges/Badge_7.png");

            migrationBuilder.UpdateData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 8,
                column: "ImagePath",
                value: "../Images/Badges/Badge_8.png");

            migrationBuilder.UpdateData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 9,
                column: "ImagePath",
                value: "../Images/Badges/Badge_9.png");

            migrationBuilder.UpdateData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "Category", "ImagePath" },
                values: new object[] { 1, "../Images/Badges/Badge_10.png" });

            migrationBuilder.UpdateData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "Category", "ImagePath" },
                values: new object[] { 1, "../Images/Badges/Badge_11.png" });

            migrationBuilder.UpdateData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 12,
                column: "ImagePath",
                value: "../Images/Badges/Badge_12.png");

            migrationBuilder.UpdateData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 13,
                column: "ImagePath",
                value: "../Images/Badges/Badge_13.png");

            migrationBuilder.UpdateData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 14,
                column: "ImagePath",
                value: "../Images/Badges/Badge_14.png");

            migrationBuilder.UpdateData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 15,
                column: "ImagePath",
                value: "../Images/Badges/Badge_15.png");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "Badges");

            migrationBuilder.UpdateData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 10,
                column: "Category",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 11,
                column: "Category",
                value: 0);
        }
    }
}
