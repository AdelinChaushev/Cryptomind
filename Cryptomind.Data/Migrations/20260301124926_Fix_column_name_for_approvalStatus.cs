using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cryptomind.Data.Migrations
{
    /// <inheritdoc />
    public partial class Fix_column_name_for_approvalStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Cipher_EncryptedText",
                table: "Cipher");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "b3a15630-f862-400f-8441-498c538c852d", "AQAAAAIAAYagAAAAEHJeR9GFpWnAeRE9YI4IzixucxWG3soslM7MS52YAstUz0eMWRxFx4Ah9Qomigepag==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b2c3d4e5-f6a7-8901-bcde-f12345678901",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "a76e6128-c778-45c0-8dc7-0a81da24ef0c", "AQAAAAIAAYagAAAAEIhVhDCRdj7saPdDBY+lVgGJsoos+N+STDKH5aVi9pazEn4hiAEZkPv61fRBLLQ21Q==" });

            migrationBuilder.CreateIndex(
                name: "IX_Cipher_EncryptedText",
                table: "Cipher",
                column: "EncryptedText",
                unique: true,
                filter: "IsDeleted = 0 AND Status != 2");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Cipher_EncryptedText",
                table: "Cipher");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "3c2178fd-2fbd-4f7b-837b-a7ef6cc72c38", "AQAAAAIAAYagAAAAEKlKonTAR1LgEjN1BVGW3cM3gOAkwbrzZz8jB20B3g10Y11jhAaWI4GTQA8HiZ1Rwg==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b2c3d4e5-f6a7-8901-bcde-f12345678901",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "a525725a-70dc-4282-8367-7359098e0683", "AQAAAAIAAYagAAAAEM7vrwDWNefF4wOrb31PTr7IhKkY8XEoDVShsiXeAy7Dy8PFHTecsjkZnw146jBdAw==" });

            migrationBuilder.CreateIndex(
                name: "IX_Cipher_EncryptedText",
                table: "Cipher",
                column: "EncryptedText",
                unique: true,
                filter: "IsDeleted = 0 AND ApprovalStatus != 2");
        }
    }
}
