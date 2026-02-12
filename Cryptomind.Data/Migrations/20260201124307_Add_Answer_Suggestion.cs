using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cryptomind.Data.Migrations
{
    /// <inheritdoc />
    public partial class Add_Answer_Suggestion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserBadge_Badge_BadgeId",
                table: "UserBadge");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Badge",
                table: "Badge");

            migrationBuilder.RenameTable(
                name: "Badge",
                newName: "Badges");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Badges",
                table: "Badges",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "AnswerSuggestions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CipherId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnswerSuggestions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnswerSuggestions_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AnswerSuggestions_Cipher_CipherId",
                        column: x => x.CipherId,
                        principalTable: "Cipher",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnswerSuggestions_CipherId",
                table: "AnswerSuggestions",
                column: "CipherId");

            migrationBuilder.CreateIndex(
                name: "IX_AnswerSuggestions_UserId",
                table: "AnswerSuggestions",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserBadge_Badges_BadgeId",
                table: "UserBadge",
                column: "BadgeId",
                principalTable: "Badges",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserBadge_Badges_BadgeId",
                table: "UserBadge");

            migrationBuilder.DropTable(
                name: "AnswerSuggestions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Badges",
                table: "Badges");

            migrationBuilder.RenameTable(
                name: "Badges",
                newName: "Badge");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Badge",
                table: "Badge",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserBadge_Badge_BadgeId",
                table: "UserBadge",
                column: "BadgeId",
                principalTable: "Badge",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
