using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cryptomind.Data.Migrations
{
    /// <inheritdoc />
    public partial class fix_seeded_data : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SolvedCount",
                table: "AspNetUsers");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "4ea6d7e9-c8ac-4f0a-83f6-e6db8cc7a966", "AQAAAAIAAYagAAAAELcqm2EYSfKDaZYv8we17H+KNVv7lLC+hyEocoEXX10SaQOgBC4t6cJszV5vYphjmA==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b2c3d4e5-f6a7-8901-bcde-f12345678901",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "905fcd84-1eba-41af-9dc7-82370ab4b930", "AQAAAAIAAYagAAAAEPwco7b5ewsjup09QvVcRjX8j7TAqeMxR/plMrwVWA2I2VTRX2H62ZQ1kh1dj1jaxw==" });

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 1,
                column: "MLPrediction",
                value: "{\"Family\":\"Substitution\",\"Type\":\"ROT13\",\"Confidence\":0.98,\"AllPredictions\":[{\"Family\":\"Substitution\",\"Type\":\"ROT13\",\"Confidence\":0.98},{\"Family\":\"Substitution\",\"Type\":\"Caesar\",\"Confidence\":0.04}]}");

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 2,
                column: "MLPrediction",
                value: "{\"Family\":\"Substitution\",\"Type\":\"Caesar\",\"Confidence\":0.97,\"AllPredictions\":[{\"Family\":\"Substitution\",\"Type\":\"Caesar\",\"Confidence\":0.97},{\"Family\":\"Substitution\",\"Type\":\"ROT13\",\"Confidence\":0.06}]}");

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 3,
                column: "MLPrediction",
                value: "{\"Family\":\"Substitution\",\"Type\":\"Atbash\",\"Confidence\":0.99,\"AllPredictions\":[{\"Family\":\"Substitution\",\"Type\":\"Atbash\",\"Confidence\":0.99},{\"Family\":\"Substitution\",\"Type\":\"SimpleSubstitution\",\"Confidence\":0.03}]}");

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 4,
                column: "MLPrediction",
                value: "{\"Family\":\"Substitution\",\"Type\":\"SimpleSubstitution\",\"Confidence\":0.94,\"AllPredictions\":[{\"Family\":\"Substitution\",\"Type\":\"SimpleSubstitution\",\"Confidence\":0.94},{\"Family\":\"Substitution\",\"Type\":\"Atbash\",\"Confidence\":0.07}]}");

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 5,
                column: "MLPrediction",
                value: "{\"Family\":\"Polyalphabetic\",\"Type\":\"Vigenere\",\"Confidence\":0.96,\"AllPredictions\":[{\"Family\":\"Polyalphabetic\",\"Type\":\"Vigenere\",\"Confidence\":0.96},{\"Family\":\"Polyalphabetic\",\"Type\":\"Autokey\",\"Confidence\":0.05}]}");

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 6,
                column: "MLPrediction",
                value: "{\"Family\":\"Polyalphabetic\",\"Type\":\"Autokey\",\"Confidence\":0.93,\"AllPredictions\":[{\"Family\":\"Polyalphabetic\",\"Type\":\"Autokey\",\"Confidence\":0.93},{\"Family\":\"Polyalphabetic\",\"Type\":\"Vigenere\",\"Confidence\":0.08}]}");

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 7,
                column: "MLPrediction",
                value: "{\"Family\":\"Polyalphabetic\",\"Type\":\"Trithemius\",\"Confidence\":0.95,\"AllPredictions\":[{\"Family\":\"Polyalphabetic\",\"Type\":\"Trithemius\",\"Confidence\":0.95},{\"Family\":\"Polyalphabetic\",\"Type\":\"Vigenere\",\"Confidence\":0.06}]}");

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 8,
                column: "MLPrediction",
                value: "{\"Family\":\"Transposition\",\"Type\":\"RailFence\",\"Confidence\":0.92,\"AllPredictions\":[{\"Family\":\"Transposition\",\"Type\":\"RailFence\",\"Confidence\":0.92},{\"Family\":\"Transposition\",\"Type\":\"Columnar\",\"Confidence\":0.09}]}");

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 9,
                column: "MLPrediction",
                value: "{\"Family\":\"Transposition\",\"Type\":\"Columnar\",\"Confidence\":0.91,\"AllPredictions\":[{\"Family\":\"Transposition\",\"Type\":\"Columnar\",\"Confidence\":0.91},{\"Family\":\"Transposition\",\"Type\":\"RailFence\",\"Confidence\":0.11}]}");

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 10,
                column: "MLPrediction",
                value: "{\"Family\":\"Encoding\",\"Type\":\"Base64\",\"Confidence\":0.99,\"AllPredictions\":[{\"Family\":\"Encoding\",\"Type\":\"Base64\",\"Confidence\":0.99},{\"Family\":\"Encoding\",\"Type\":\"Hex\",\"Confidence\":0.02}]}");

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 11,
                column: "MLPrediction",
                value: "{\"Family\":\"Encoding\",\"Type\":\"Morse\",\"Confidence\":0.98,\"AllPredictions\":[{\"Family\":\"Encoding\",\"Type\":\"Morse\",\"Confidence\":0.98},{\"Family\":\"Encoding\",\"Type\":\"Binary\",\"Confidence\":0.03}]}");

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 12,
                column: "MLPrediction",
                value: "{\"Family\":\"Encoding\",\"Type\":\"Binary\",\"Confidence\":0.97,\"AllPredictions\":[{\"Family\":\"Encoding\",\"Type\":\"Binary\",\"Confidence\":0.97},{\"Family\":\"Encoding\",\"Type\":\"Hex\",\"Confidence\":0.04}]}");

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 13,
                column: "MLPrediction",
                value: "{\"Family\":\"Encoding\",\"Type\":\"Hex\",\"Confidence\":0.98,\"AllPredictions\":[{\"Family\":\"Encoding\",\"Type\":\"Hex\",\"Confidence\":0.98},{\"Family\":\"Encoding\",\"Type\":\"Base64\",\"Confidence\":0.03}]}");

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 14,
                column: "MLPrediction",
                value: "{\"Family\":\"Substitution\",\"Type\":\"Caesar\",\"Confidence\":0.96,\"AllPredictions\":[{\"Family\":\"Substitution\",\"Type\":\"Caesar\",\"Confidence\":0.96},{\"Family\":\"Substitution\",\"Type\":\"ROT13\",\"Confidence\":0.05}]}");

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 15,
                column: "MLPrediction",
                value: "{\"Family\":\"Polyalphabetic\",\"Type\":\"Vigenere\",\"Confidence\":0.89,\"AllPredictions\":[{\"Family\":\"Polyalphabetic\",\"Type\":\"Vigenere\",\"Confidence\":0.89},{\"Family\":\"Polyalphabetic\",\"Type\":\"Autokey\",\"Confidence\":0.14},{\"Family\":\"Substitution\",\"Type\":\"SimpleSubstitution\",\"Confidence\":0.07}]}");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SolvedCount",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SolvedCount" },
                values: new object[] { "138df409-428e-495d-8f80-6802d6604618", "AQAAAAIAAYagAAAAELQU/H9FBYfccBKf+VoQ04UWAn0sZyBdkFo7G9prxZhbgC3fRgm8qd3oxq4LiwTvzw==", 0 });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b2c3d4e5-f6a7-8901-bcde-f12345678901",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SolvedCount" },
                values: new object[] { "9f8f0f10-1b64-4f2a-822f-638f65f18221", "AQAAAAIAAYagAAAAEF3mwZoDXYvOoKr4bQrxTCaVPbfYD4XfmuTO1J3AwJRo+KYWedkpcXWjwOzkupNMFg==", 0 });

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 1,
                column: "MLPrediction",
                value: "ROT13");

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 2,
                column: "MLPrediction",
                value: "Caesar");

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 3,
                column: "MLPrediction",
                value: "Atbash");

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 4,
                column: "MLPrediction",
                value: "SimpleSubstitution");

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 5,
                column: "MLPrediction",
                value: "Vigenere");

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 6,
                column: "MLPrediction",
                value: "Autokey");

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 7,
                column: "MLPrediction",
                value: "Trithemius");

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 8,
                column: "MLPrediction",
                value: "RailFence");

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 9,
                column: "MLPrediction",
                value: "Columnar");

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 10,
                column: "MLPrediction",
                value: "Base64");

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 11,
                column: "MLPrediction",
                value: "Morse");

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 12,
                column: "MLPrediction",
                value: "Binary");

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 13,
                column: "MLPrediction",
                value: "Hex");

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 14,
                column: "MLPrediction",
                value: "Caesar");

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 15,
                column: "MLPrediction",
                value: "Vigenere");
        }
    }
}
