using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Cryptomind.Data.Migrations
{
    /// <inheritdoc />
    public partial class BadgeSeeding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Category", "Description", "Title" },
                values: new object[] { 0, "Solve 50 ciphers", "Seasoned Decoder" });

            migrationBuilder.UpdateData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Category", "Description", "Title" },
                values: new object[] { 0, "Solve 100 ciphers", "Master Cryptanalyst" });

            migrationBuilder.UpdateData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Category", "Description" },
                values: new object[] { 0, "Solve ciphers from 5 different types" });

            migrationBuilder.UpdateData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Category", "Description", "Title" },
                values: new object[] { 0, "Solve ciphers from 10 different types", "Polyglot Decoder" });

            migrationBuilder.InsertData(
                table: "Badges",
                columns: new[] { "Id", "Category", "Description", "EarnedBy", "Title" },
                values: new object[,]
                {
                    { 7, 2, "Have your first cipher approved", 0, "Cipher Creator" },
                    { 8, 2, "Have 5 ciphers approved", 0, "Community Contributor" },
                    { 9, 2, "Have 15 ciphers approved", 0, "Architect of Ciphers" },
                    { 10, 0, "First approved suggested answer", 0, "Helpful Mind" },
                    { 11, 0, "10 approved suggested answers", 0, "Trusted Contributor" },
                    { 12, 0, "Solve 10 ciphers without using hints", 0, "No Mercy" },
                    { 13, 0, "Solve 10 ciphers correctly on the first attempt", 0, "Flawless Solver" },
                    { 14, 0, "Use hints on 25 different ciphers", 0, "Curious Mind" },
                    { 15, 0, "Solve a cipher solved by fewer than 3 users", 0, "Against the Odds" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.UpdateData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Category", "Description", "Title" },
                values: new object[] { 2, "Have your first cipher approved", "Cipher Creator" });

            migrationBuilder.UpdateData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Category", "Description", "Title" },
                values: new object[] { 2, "Have 5 ciphers approved", "Community Contributor" });

            migrationBuilder.UpdateData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Category", "Description" },
                values: new object[] { 3, "Solve at least one cipher from 5 different types" });

            migrationBuilder.UpdateData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Category", "Description", "Title" },
                values: new object[] { 1, "Solve your first experimental cipher", "Outstanding Cryptographer" });
        }
    }
}
