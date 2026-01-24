using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cryptomind.Data.Migrations
{
    /// <inheritdoc />
    public partial class Update_Badge_for_EarnedBy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EarnedBy",
                table: "Badge",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Badge",
                keyColumn: "Id",
                keyValue: 1,
                column: "EarnedBy",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Badge",
                keyColumn: "Id",
                keyValue: 2,
                column: "EarnedBy",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Badge",
                keyColumn: "Id",
                keyValue: 3,
                column: "EarnedBy",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Badge",
                keyColumn: "Id",
                keyValue: 4,
                column: "EarnedBy",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Badge",
                keyColumn: "Id",
                keyValue: 5,
                column: "EarnedBy",
                value: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EarnedBy",
                table: "Badge");
        }
    }
}
