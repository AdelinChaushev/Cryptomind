using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cryptomind.Data.Migrations
{
    /// <inheritdoc />
    public partial class Update_for_AnswerSuggestions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DecryptedText",
                table: "AnswerSuggestions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "AnswerSuggestions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UplodaedTime",
                table: "AnswerSuggestions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 3,
                column: "Category",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 4,
                column: "Category",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 5,
                column: "Category",
                value: 3);

            migrationBuilder.InsertData(
                table: "Badges",
                columns: new[] { "Id", "Category", "Description", "EarnedBy", "Title" },
                values: new object[] { 6, 1, "Solve your first experimental cipher", 0, "Outstanding Cryptographer" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DropColumn(
                name: "DecryptedText",
                table: "AnswerSuggestions");

            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "AnswerSuggestions");

            migrationBuilder.DropColumn(
                name: "UplodaedTime",
                table: "AnswerSuggestions");

            migrationBuilder.UpdateData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 3,
                column: "Category",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 4,
                column: "Category",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 5,
                column: "Category",
                value: 2);
        }
    }
}
