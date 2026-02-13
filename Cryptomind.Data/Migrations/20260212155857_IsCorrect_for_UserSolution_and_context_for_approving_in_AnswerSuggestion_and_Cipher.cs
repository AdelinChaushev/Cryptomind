using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cryptomind.Data.Migrations
{
    /// <inheritdoc />
    public partial class IsCorrect_for_UserSolution_and_context_for_approving_in_AnswerSuggestion_and_Cipher : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCorrect",
                table: "UserSolution",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedAt",
                table: "Cipher",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RejectedAt",
                table: "Cipher",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RejectionReason",
                table: "Cipher",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovalDate",
                table: "AnswerSuggestions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PointsEarned",
                table: "AnswerSuggestions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "RejectionDate",
                table: "AnswerSuggestions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RejectionReason",
                table: "AnswerSuggestions",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCorrect",
                table: "UserSolution");

            migrationBuilder.DropColumn(
                name: "ApprovedAt",
                table: "Cipher");

            migrationBuilder.DropColumn(
                name: "RejectedAt",
                table: "Cipher");

            migrationBuilder.DropColumn(
                name: "RejectionReason",
                table: "Cipher");

            migrationBuilder.DropColumn(
                name: "ApprovalDate",
                table: "AnswerSuggestions");

            migrationBuilder.DropColumn(
                name: "PointsEarned",
                table: "AnswerSuggestions");

            migrationBuilder.DropColumn(
                name: "RejectionDate",
                table: "AnswerSuggestions");

            migrationBuilder.DropColumn(
                name: "RejectionReason",
                table: "AnswerSuggestions");
        }
    }
}
