using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cryptomind.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDailyChallenge : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CurrentStreak",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LongestStreak",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "DailyChallengeEntries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlainText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EncryptedText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CipherType = table.Column<int>(type: "int", nullable: false),
                    IsUsed = table.Column<bool>(type: "bit", nullable: false),
                    AssignedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyChallengeEntries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DailyChallengeParticipations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DailyChallengeEntryId = table.Column<int>(type: "int", nullable: false),
                    ChallengeDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    SolvedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AttemptCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyChallengeParticipations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DailyChallengeParticipations_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DailyChallengeParticipations_DailyChallengeEntries_DailyChallengeEntryId",
                        column: x => x.DailyChallengeEntryId,
                        principalTable: "DailyChallengeEntries",
                        principalColumn: "Id");
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
                columns: new[] { "ConcurrencyStamp", "CurrentStreak", "LongestStreak", "PasswordHash" },
                values: new object[] { "2f310e91-4eb4-4ef5-977b-bca6682503da", 0, 0, "AQAAAAIAAYagAAAAEPfUDE6nbFcM0i99V1lbGyLgTx5FhoBb8HkFeCsiumdi7JUgTo0KU1TIVtkBqLN+yQ==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b2c3d4e5-f6a7-8901-bcde-f12345678901",
                columns: new[] { "ConcurrencyStamp", "CurrentStreak", "LongestStreak", "PasswordHash" },
                values: new object[] { "6b29799a-af8e-405e-b9ce-f323e48b363c", 0, 0, "AQAAAAIAAYagAAAAECUU+oPsPOvwuR5yQr6yxLHBrmqj6LJ+ainWe0JxxyDHi8i2SWr8QX00guvIfFrsHQ==" });

            migrationBuilder.CreateIndex(
                name: "IX_DailyChallengeEntries_AssignedDate",
                table: "DailyChallengeEntries",
                column: "AssignedDate",
                unique: true,
                filter: "[AssignedDate] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_DailyChallengeParticipations_DailyChallengeEntryId",
                table: "DailyChallengeParticipations",
                column: "DailyChallengeEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_DailyChallengeParticipations_UserId_ChallengeDate",
                table: "DailyChallengeParticipations",
                columns: new[] { "UserId", "ChallengeDate" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DailyChallengeParticipations");

            migrationBuilder.DropTable(
                name: "DailyChallengeEntries");

            migrationBuilder.DropColumn(
                name: "CurrentStreak",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LongestStreak",
                table: "AspNetUsers");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "1c77bf7b-5080-4250-af44-1a24fee62f72", "AQAAAAIAAYagAAAAEKIyyYnqsa/wtdUkIN9SaiKBMxtaGI+MMbCy3pl/2BZrtOdM221ThCHpbesr2RlsWw==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b2c3d4e5-f6a7-8901-bcde-f12345678901",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "535594f3-f335-4f63-80ea-58575d7e0c43", "AQAAAAIAAYagAAAAEH3iz2Y2bjfCI6P8PHqwJxCp1hxMUHmeDiiFW3i5qP9JqJn9hfsJPafZ+eWEBdZR6Q==" });
        }
    }
}
