using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cryptomind.Data.Migrations
{
    /// <inheritdoc />
    public partial class Add_DisplayOrder_for_shuffling : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DisplayOrder",
                table: "Cipher",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "f4af1202-bb9e-46fb-8a31-57247038748d", "AQAAAAIAAYagAAAAEEaGeD2lolU5SnG3G5JD3vYTD1MwNEKsFHtWfnc9hEoKVxSxggFWToM3JnRPIrQkJQ==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b2c3d4e5-f6a7-8901-bcde-f12345678901",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "6b5d4063-62b0-427c-9ea2-b8971a17f9bc", "AQAAAAIAAYagAAAAEGTatZbTKk+VyoPJ2nS1aRDJ6Zrt0JDHg7EOBCypjIeiEqWeMSTiC74BirViNGpVEg==" });

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 1,
                column: "DisplayOrder",
                value: 9);

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 2,
                column: "DisplayOrder",
                value: 20);

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 3,
                column: "DisplayOrder",
                value: 16);

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 4,
                column: "DisplayOrder",
                value: 6);

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 5,
                column: "DisplayOrder",
                value: 35);

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 6,
                column: "DisplayOrder",
                value: 25);

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 7,
                column: "DisplayOrder",
                value: 3);

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 8,
                column: "DisplayOrder",
                value: 13);

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 9,
                column: "DisplayOrder",
                value: 30);

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 10,
                column: "DisplayOrder",
                value: 8);

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 11,
                column: "DisplayOrder",
                value: 48);

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 12,
                column: "DisplayOrder",
                value: 22);

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 13,
                column: "DisplayOrder",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 14,
                column: "DisplayOrder",
                value: 39);

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 15,
                column: "DisplayOrder",
                value: 27);

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 16,
                column: "DisplayOrder",
                value: 11);

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 17,
                column: "DisplayOrder",
                value: 29);

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 18,
                column: "DisplayOrder",
                value: 43);

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 19,
                column: "DisplayOrder",
                value: 15);

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 20,
                column: "DisplayOrder",
                value: 34);

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 21,
                column: "DisplayOrder",
                value: 5);

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 22,
                column: "DisplayOrder",
                value: 37);

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 23,
                column: "DisplayOrder",
                value: 19);

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 24,
                column: "DisplayOrder",
                value: 44);

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 25,
                column: "DisplayOrder",
                value: 12);

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 26,
                column: "DisplayOrder",
                value: 28);

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 27,
                column: "DisplayOrder",
                value: 47);

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 28,
                column: "DisplayOrder",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 29,
                column: "DisplayOrder",
                value: 42);

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 30,
                column: "DisplayOrder",
                value: 18);

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 31,
                column: "DisplayOrder",
                value: 41);

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 32,
                column: "DisplayOrder",
                value: 24);

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 33,
                column: "DisplayOrder",
                value: 31);

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 34,
                column: "DisplayOrder",
                value: 38);

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 35,
                column: "DisplayOrder",
                value: 4);

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 36,
                column: "DisplayOrder",
                value: 49);

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 37,
                column: "DisplayOrder",
                value: 21);

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 38,
                column: "DisplayOrder",
                value: 46);

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 39,
                column: "DisplayOrder",
                value: 14);

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 40,
                column: "DisplayOrder",
                value: 40);

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 41,
                column: "DisplayOrder",
                value: 32);

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 42,
                column: "DisplayOrder",
                value: 10);

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 43,
                column: "DisplayOrder",
                value: 50);

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 44,
                column: "DisplayOrder",
                value: 23);

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 45,
                column: "DisplayOrder",
                value: 45);

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 46,
                column: "DisplayOrder",
                value: 17);

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 47,
                column: "DisplayOrder",
                value: 7);

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 48,
                column: "DisplayOrder",
                value: 36);

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 49,
                column: "DisplayOrder",
                value: 33);

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 50,
                column: "DisplayOrder",
                value: 26);

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 51,
                column: "DisplayOrder",
                value: 51);

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 52,
                column: "DisplayOrder",
                value: 52);

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 53,
                column: "DisplayOrder",
                value: 53);

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 54,
                column: "DisplayOrder",
                value: 54);

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 55,
                column: "DisplayOrder",
                value: 55);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisplayOrder",
                table: "Cipher");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "cfd419f5-a846-457d-a22e-5ed192397c8b", "AQAAAAIAAYagAAAAEFsuUolzDHgBIuL759QHou/KXtfUVirH/r3URMvtmjCRkrnLwUhcP5GF1hlOg6WCRQ==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b2c3d4e5-f6a7-8901-bcde-f12345678901",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "5dc3fadf-e09c-4d1a-ba57-de8e4f88f6c2", "AQAAAAIAAYagAAAAEK/VZQ2Dp6W3nqBWmBH4sBybgAFaItnOEYXuwJnN/tJxWuZxQfGbkubs3ne2gywBVA==" });
        }
    }
}
