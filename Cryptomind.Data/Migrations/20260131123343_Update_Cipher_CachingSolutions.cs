using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cryptomind.Data.Migrations
{
    /// <inheritdoc />
    public partial class Update_Cipher_CachingSolutions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TextCipher_EncryptedText",
                table: "Cipher",
                newName: "CachedSolution");

            migrationBuilder.AlterColumn<string>(
                name: "EncryptedText",
                table: "Cipher",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CachedHint",
                table: "Cipher",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CachedHint",
                table: "Cipher");

            migrationBuilder.RenameColumn(
                name: "CachedSolution",
                table: "Cipher",
                newName: "TextCipher_EncryptedText");

            migrationBuilder.AlterColumn<string>(
                name: "EncryptedText",
                table: "Cipher",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
