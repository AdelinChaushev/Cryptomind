using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cryptomind.Data.Migrations
{
    /// <inheritdoc />
    public partial class Update_ApplicationUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Cipher_EncryptedText",
                table: "Cipher");

            migrationBuilder.DropColumn(
                name: "AttemptedCiphers",
                table: "AspNetUsers");

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

            migrationBuilder.UpdateData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 1,
                column: "Description",
                value: "Реши първия си шифър");

            migrationBuilder.UpdateData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 2,
                column: "Description",
                value: "Реши 25 шифъра");

            migrationBuilder.UpdateData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 3,
                column: "Description",
                value: "Реши 50 шифъра");

            migrationBuilder.UpdateData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 4,
                column: "Description",
                value: "Реши 100 шифъра");

            migrationBuilder.UpdateData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 5,
                column: "Description",
                value: "Реши шифри от 5 различни вида");

            migrationBuilder.UpdateData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 6,
                column: "Description",
                value: "Реши шифри от 10 различни вида");

            migrationBuilder.UpdateData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 7,
                column: "Description",
                value: "Получи одобрение за първия си шифър");

            migrationBuilder.UpdateData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 8,
                column: "Description",
                value: "Получи одобрение за 5 предложения за шифри");

            migrationBuilder.UpdateData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 9,
                column: "Description",
                value: "Получи одобрение за 15 предложения за шифри");

            migrationBuilder.UpdateData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 10,
                column: "Description",
                value: "Първи одобрен предложен отговор");

            migrationBuilder.UpdateData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 11,
                column: "Description",
                value: "10 одобрени предложени отговора");

            migrationBuilder.UpdateData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 12,
                column: "Description",
                value: "Реши 10 шифъра без да използваш подсказки");

            migrationBuilder.UpdateData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 13,
                column: "Description",
                value: "Реши 10 шифъра правилно от първия опит");

            migrationBuilder.UpdateData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 14,
                column: "Description",
                value: "Използвай подсказки на 25 различни шифъра");

            migrationBuilder.UpdateData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 15,
                column: "Description",
                value: "Реши шифър, решен от по-малко от 3 потребители");

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

            migrationBuilder.AddColumn<int>(
                name: "AttemptedCiphers",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
                columns: new[] { "AttemptedCiphers", "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { 0, "aa74da97-7e16-4bcf-beae-aa307f24a016", "AQAAAAIAAYagAAAAEB7MqnzQt5TkIe8xj1jhC75uDBUiRGKPbtuUlJKrfAjVhMcGrVwIAaX/c9P/v4CWKw==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b2c3d4e5-f6a7-8901-bcde-f12345678901",
                columns: new[] { "AttemptedCiphers", "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { 0, "d3bb9f58-c28b-4960-83d2-a57a9cc5e641", "AQAAAAIAAYagAAAAEI3KNEEgM0Su6xBSwMKxTyE7XV3FrU2jmbuXLYfyrZp3EzDjrmEmvSjTWQrAPztbqQ==" });

            migrationBuilder.UpdateData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 1,
                column: "Description",
                value: "Solve your first cipher");

            migrationBuilder.UpdateData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 2,
                column: "Description",
                value: "Solve 25 ciphers");

            migrationBuilder.UpdateData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 3,
                column: "Description",
                value: "Solve 50 ciphers");

            migrationBuilder.UpdateData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 4,
                column: "Description",
                value: "Solve 100 ciphers");

            migrationBuilder.UpdateData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 5,
                column: "Description",
                value: "Solve ciphers from 5 different types");

            migrationBuilder.UpdateData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 6,
                column: "Description",
                value: "Solve ciphers from 10 different types");

            migrationBuilder.UpdateData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 7,
                column: "Description",
                value: "Have your first cipher approved");

            migrationBuilder.UpdateData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 8,
                column: "Description",
                value: "Have 5 ciphers approved");

            migrationBuilder.UpdateData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 9,
                column: "Description",
                value: "Have 15 ciphers approved");

            migrationBuilder.UpdateData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 10,
                column: "Description",
                value: "First approved suggested answer");

            migrationBuilder.UpdateData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 11,
                column: "Description",
                value: "10 approved suggested answers");

            migrationBuilder.UpdateData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 12,
                column: "Description",
                value: "Solve 10 ciphers without using hints");

            migrationBuilder.UpdateData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 13,
                column: "Description",
                value: "Solve 10 ciphers correctly on the first attempt");

            migrationBuilder.UpdateData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 14,
                column: "Description",
                value: "Use hints on 25 different ciphers");

            migrationBuilder.UpdateData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 15,
                column: "Description",
                value: "Solve a cipher solved by fewer than 3 users");

            migrationBuilder.CreateIndex(
                name: "IX_Cipher_EncryptedText",
                table: "Cipher",
                column: "EncryptedText",
                unique: true);
        }
    }
}
