using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cryptomind.Data.Migrations
{
    /// <inheritdoc />
    public partial class UserCipherMappingTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cipher_AspNetUsers_CreatedByUserId",
                table: "Cipher");

            migrationBuilder.AddColumn<int>(
                name: "Points",
                table: "Cipher",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "UserSolution",
                columns: table => new
                {
                    CipherId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TimeSolved = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSolution", x => new { x.CipherId, x.UserId });
                    table.ForeignKey(
                        name: "FK_UserSolution_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserSolution_Cipher_CipherId",
                        column: x => x.CipherId,
                        principalTable: "Cipher",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserSolution_UserId",
                table: "UserSolution",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cipher_AspNetUsers_CreatedByUserId",
                table: "Cipher",
                column: "CreatedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cipher_AspNetUsers_CreatedByUserId",
                table: "Cipher");

            migrationBuilder.DropTable(
                name: "UserSolution");

            migrationBuilder.DropColumn(
                name: "Points",
                table: "Cipher");

            migrationBuilder.AddForeignKey(
                name: "FK_Cipher_AspNetUsers_CreatedByUserId",
                table: "Cipher",
                column: "CreatedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
