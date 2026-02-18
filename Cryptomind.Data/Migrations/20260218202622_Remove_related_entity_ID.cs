using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cryptomind.Data.Migrations
{
    /// <inheritdoc />
    public partial class Remove_related_entity_ID : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserSolution_UserId",
                table: "UserSolution");

            migrationBuilder.DropIndex(
                name: "IX_UserBadge_UserId",
                table: "UserBadge");

            migrationBuilder.DropColumn(
                name: "RelatedEntityId",
                table: "Notifications");

            migrationBuilder.CreateIndex(
                name: "IX_UserSolution_UserId_CipherId",
                table: "UserSolution",
                columns: new[] { "UserId", "CipherId" },
                unique: true,
                filter: "[IsCorrect] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_UserBadge_UserId_BadgeId",
                table: "UserBadge",
                columns: new[] { "UserId", "BadgeId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserSolution_UserId_CipherId",
                table: "UserSolution");

            migrationBuilder.DropIndex(
                name: "IX_UserBadge_UserId_BadgeId",
                table: "UserBadge");

            migrationBuilder.AddColumn<int>(
                name: "RelatedEntityId",
                table: "Notifications",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserSolution_UserId",
                table: "UserSolution",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserBadge_UserId",
                table: "UserBadge",
                column: "UserId");
        }
    }
}
