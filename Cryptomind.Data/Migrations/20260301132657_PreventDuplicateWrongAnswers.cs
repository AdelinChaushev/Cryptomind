using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cryptomind.Data.Migrations
{
    /// <inheritdoc />
    public partial class PreventDuplicateWrongAnswers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Solution",
                table: "UserSolution",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

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

            migrationBuilder.CreateIndex(
                name: "IX_UserSolution_UserId_CipherId_Solution",
                table: "UserSolution",
                columns: new[] { "UserId", "CipherId", "Solution" },
                unique: true,
                filter: "[IsCorrect] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserSolution_UserId_CipherId_Solution",
                table: "UserSolution");

            migrationBuilder.AlterColumn<string>(
                name: "Solution",
                table: "UserSolution",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "a3ffaea8-d9ae-4833-a5ec-0f6b1e80ab76", "AQAAAAIAAYagAAAAEFs7Z1YpB+fLnnG8LrgXWYlFWgAI0SqKCx0H9AQXQ2lxWdntLKAWzmm/CK2EISC3Wg==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b2c3d4e5-f6a7-8901-bcde-f12345678901",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "ada7ce56-ba71-43aa-9098-a7dfbcd06964", "AQAAAAIAAYagAAAAEF8PefMvtYEwNWRi2JQvZFIEmy7OPvarLAXwaqbWDiAWyuWOZBmRT1nXzX2zm6XKPA==" });
        }
    }
}
