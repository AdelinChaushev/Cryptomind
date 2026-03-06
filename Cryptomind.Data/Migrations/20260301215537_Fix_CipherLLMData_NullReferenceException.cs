using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cryptomind.Data.Migrations
{
    /// <inheritdoc />
    public partial class Fix_CipherLLMData_NullReferenceException : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "LLMData_CachedTypeHint",
                table: "Cipher",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LLMData_CachedSolution",
                table: "Cipher",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LLMData_CachedHint",
                table: "Cipher",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "46cca603-d3aa-4a3a-a49d-317955f2dc8e", "AQAAAAIAAYagAAAAEAGO3AGqqltYO/p7GiszvifA+8C7Mc+1wuiPsKFE+M3fMbbDFNKjz/oRAFSFoZNINA==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b2c3d4e5-f6a7-8901-bcde-f12345678901",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "82111097-d613-49f1-9769-f6bb9b678de1", "AQAAAAIAAYagAAAAEBNVSb4lychzF5CzKMzOfxC8rwz1XPzYgdQm+1BzpLS6rJk3BmEfi8PKrDCNKuX7dA==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "LLMData_CachedTypeHint",
                table: "Cipher",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "LLMData_CachedSolution",
                table: "Cipher",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "LLMData_CachedHint",
                table: "Cipher",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "817bae33-e383-4e05-ad99-b5554f9a9ffe", "AQAAAAIAAYagAAAAEB9jEG5vDtGr5xPNF9b8FJfPl7D2MMGnhg14TD5du9hZF1W/IAZY9lbxwAHE3utPzw==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b2c3d4e5-f6a7-8901-bcde-f12345678901",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "7137b4e7-9c81-41cd-98a1-65cddfa64ddf", "AQAAAAIAAYagAAAAEPM62suPQEvWEWCBlg3TqAMd6I+8RYHAlwVyNxu8160+Ohy2TRd4DXvqEn9YJfTBtg==" });
        }
    }
}
