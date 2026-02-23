using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cryptomind.Data.Migrations
{
    /// <inheritdoc />
    public partial class Add_Validation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AnswerSuggestions_UserId",
                table: "AnswerSuggestions");

            migrationBuilder.AlterColumn<string>(
                name: "EncryptedText",
                table: "Cipher",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "DecryptedText",
                table: "AnswerSuggestions",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Cipher_EncryptedText",
                table: "Cipher",
                column: "EncryptedText",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cipher_Title",
                table: "Cipher",
                column: "Title",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AnswerSuggestions_UserId_CipherId_DecryptedText",
                table: "AnswerSuggestions",
                columns: new[] { "UserId", "CipherId", "DecryptedText" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Cipher_EncryptedText",
                table: "Cipher");

            migrationBuilder.DropIndex(
                name: "IX_Cipher_Title",
                table: "Cipher");

            migrationBuilder.DropIndex(
                name: "IX_AnswerSuggestions_UserId_CipherId_DecryptedText",
                table: "AnswerSuggestions");

            migrationBuilder.AlterColumn<string>(
                name: "EncryptedText",
                table: "Cipher",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "DecryptedText",
                table: "AnswerSuggestions",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateIndex(
                name: "IX_AnswerSuggestions_UserId",
                table: "AnswerSuggestions",
                column: "UserId");
        }
    }
}
