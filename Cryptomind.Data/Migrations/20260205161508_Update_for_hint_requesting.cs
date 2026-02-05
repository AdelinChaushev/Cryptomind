using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cryptomind.Data.Migrations
{
    /// <inheritdoc />
    public partial class Update_for_hint_requesting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PointsEarned",
                table: "UserSolution",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "UsedFullSolution",
                table: "UserSolution",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "UsedSolutionHint",
                table: "UserSolution",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "UsedTypeHint",
                table: "UserSolution",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "HintContent",
                table: "HintRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "RequestedAt",
                table: "HintRequests",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PointsEarned",
                table: "UserSolution");

            migrationBuilder.DropColumn(
                name: "UsedFullSolution",
                table: "UserSolution");

            migrationBuilder.DropColumn(
                name: "UsedSolutionHint",
                table: "UserSolution");

            migrationBuilder.DropColumn(
                name: "UsedTypeHint",
                table: "UserSolution");

            migrationBuilder.DropColumn(
                name: "HintContent",
                table: "HintRequests");

            migrationBuilder.DropColumn(
                name: "RequestedAt",
                table: "HintRequests");
        }
    }
}
