using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cryptomind.Data.Migrations
{
    /// <inheritdoc />
    public partial class Updated_LLMDATA : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LLMData_Recommendation",
                table: "Cipher",
                type: "nvarchar(max)",
                nullable: true);

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

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 1,
                column: "LLMData_Recommendation",
                value: "Approve");

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 2,
                column: "LLMData_Recommendation",
                value: "Approve");

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 3,
                column: "LLMData_Recommendation",
                value: "Approve");

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 4,
                column: "LLMData_Recommendation",
                value: "Approve");

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 5,
                column: "LLMData_Recommendation",
                value: "Approve");

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 6,
                column: "LLMData_Recommendation",
                value: "Approve");

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 7,
                column: "LLMData_Recommendation",
                value: "Approve");

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 8,
                column: "LLMData_Recommendation",
                value: "Approve");

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 9,
                column: "LLMData_Recommendation",
                value: "Approve");

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 10,
                column: "LLMData_Recommendation",
                value: "Approve");

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 11,
                column: "LLMData_Recommendation",
                value: "Approve");

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 12,
                column: "LLMData_Recommendation",
                value: "Approve");

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 13,
                column: "LLMData_Recommendation",
                value: "Approve");

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 14,
                column: "LLMData_Recommendation",
                value: "Approve");

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 15,
                column: "LLMData_Recommendation",
                value: "Approve");

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 16,
                column: "LLMData_Recommendation",
                value: "Approve");

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 17,
                column: "LLMData_Recommendation",
                value: "Approve");

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 18,
                column: "LLMData_Recommendation",
                value: "Approve");

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 19,
                column: "LLMData_Recommendation",
                value: "Approve");

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 20,
                column: "LLMData_Recommendation",
                value: "Approve");

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 21,
                column: "LLMData_Recommendation",
                value: "Approve");

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 22,
                column: "LLMData_Recommendation",
                value: "Approve");

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 23,
                column: "LLMData_Recommendation",
                value: "Approve");

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 24,
                column: "LLMData_Recommendation",
                value: "Approve");

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 25,
                column: "LLMData_Recommendation",
                value: "Approve");

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 26,
                column: "LLMData_Recommendation",
                value: "Approve");

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 27,
                column: "LLMData_Recommendation",
                value: "Approve");

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 28,
                column: "LLMData_Recommendation",
                value: "Approve");

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 29,
                column: "LLMData_Recommendation",
                value: "Approve");

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 30,
                column: "LLMData_Recommendation",
                value: "Approve");

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 31,
                column: "LLMData_Recommendation",
                value: "Approve");

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 32,
                column: "LLMData_Recommendation",
                value: "Approve");

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 33,
                column: "LLMData_Recommendation",
                value: "Approve");

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 34,
                column: "LLMData_Recommendation",
                value: "Approve");

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 35,
                column: "LLMData_Recommendation",
                value: "Approve");

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 36,
                column: "LLMData_Recommendation",
                value: "Approve");

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 37,
                column: "LLMData_Recommendation",
                value: "Approve");

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 38,
                column: "LLMData_Recommendation",
                value: "Approve");

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 39,
                column: "LLMData_Recommendation",
                value: "Approve");

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 40,
                column: "LLMData_Recommendation",
                value: "Approve");

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 41,
                column: "LLMData_Recommendation",
                value: "Approve");

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 42,
                column: "LLMData_Recommendation",
                value: "Approve");

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 43,
                column: "LLMData_Recommendation",
                value: "Approve");

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 44,
                column: "LLMData_Recommendation",
                value: "Approve");

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 45,
                column: "LLMData_Recommendation",
                value: "Approve");

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 46,
                column: "LLMData_Recommendation",
                value: "Approve");

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 47,
                column: "LLMData_Recommendation",
                value: "Approve");

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 48,
                column: "LLMData_Recommendation",
                value: "Approve");

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 49,
                column: "LLMData_Recommendation",
                value: "Approve");

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 50,
                column: "LLMData_Recommendation",
                value: "Approve");

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 51,
                column: "LLMData_Recommendation",
                value: "Approve");

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 52,
                column: "LLMData_Recommendation",
                value: "Approve");

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 53,
                column: "LLMData_Recommendation",
                value: "Approve");

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 54,
                column: "LLMData_Recommendation",
                value: "Approve");

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 55,
                column: "LLMData_Recommendation",
                value: "Approve");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LLMData_Recommendation",
                table: "Cipher");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "0fea5c1f-3a61-40f1-b176-417bf1089f88", "AQAAAAIAAYagAAAAEPsttWeceXIx4AA3MuRdtEts0dVyiesAAPpLEbvMIwagBqWtKQAhx4/g20Qu5JS9xQ==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b2c3d4e5-f6a7-8901-bcde-f12345678901",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "78dcec41-4d08-4e7b-b85b-202f420ebff1", "AQAAAAIAAYagAAAAEH3mHrzna4lcFH2BDTCg7hXpAq7QHCUJgxQPb2ue143AunW5UvH5BBZWK4WJhLaldg==" });
        }
    }
}
