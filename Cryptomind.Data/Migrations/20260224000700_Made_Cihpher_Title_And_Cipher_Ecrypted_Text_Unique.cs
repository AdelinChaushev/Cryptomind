using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cryptomind.Data.Migrations
{
    /// <inheritdoc />
    public partial class Made_Cihpher_Title_And_Cipher_Ecrypted_Text_Unique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "EncryptedText",
                table: "Cipher",
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
                unique: true,
                filter: "IsDeleted = 0");
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

            migrationBuilder.AlterColumn<string>(
                name: "EncryptedText",
                table: "Cipher",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
