using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cryptomind.Data.Migrations
{
    /// <inheritdoc />
    public partial class ApprovalStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "Cipher");

            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "AnswerSuggestions");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Cipher",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "AnswerSuggestions",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Cipher");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "AnswerSuggestions");

            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "Cipher",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "AnswerSuggestions",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
