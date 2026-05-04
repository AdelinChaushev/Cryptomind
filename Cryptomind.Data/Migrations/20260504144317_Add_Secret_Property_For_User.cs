using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cryptomind.Data.Migrations
{
    /// <inheritdoc />
    public partial class Add_Secret_Property_For_User : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasRevealedSecret",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
                columns: new[] { "ConcurrencyStamp", "HasRevealedSecret", "PasswordHash" },
                values: new object[] { "651673f4-eb77-42bb-b77f-0ca268129d4b", false, "AQAAAAIAAYagAAAAENjOUyq/DWb/KvUd/gQSkDizpAIPmB+bYgXGVmHXYo8hXoVwaIGyZ0SkawFlWMyO2w==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b2c3d4e5-f6a7-8901-bcde-f12345678901",
                columns: new[] { "ConcurrencyStamp", "HasRevealedSecret", "PasswordHash" },
                values: new object[] { "438c369b-781e-4bf5-aef6-3618ef25e3ee", false, "AQAAAAIAAYagAAAAEO2lVlSeRFDNxcmtb3OUZXvTGYKo2a3jpvk+yW42xegC3cvhzQiLU00nCDca7YtF4Q==" });

            migrationBuilder.InsertData(
                table: "Badges",
                columns: new[] { "Id", "Category", "Description", "EarnedBy", "ImagePath", "Title" },
                values: new object[] { 16, 4, "Открий скритото съдържание чрез ултравиолетова светлина", 0, "../Images/Badges/Badge_16.png", "Luminous Secret" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DropColumn(
                name: "HasRevealedSecret",
                table: "AspNetUsers");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "2f310e91-4eb4-4ef5-977b-bca6682503da", "AQAAAAIAAYagAAAAEPfUDE6nbFcM0i99V1lbGyLgTx5FhoBb8HkFeCsiumdi7JUgTo0KU1TIVtkBqLN+yQ==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b2c3d4e5-f6a7-8901-bcde-f12345678901",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "6b29799a-af8e-405e-b9ce-f323e48b363c", "AQAAAAIAAYagAAAAECUU+oPsPOvwuR5yQr6yxLHBrmqj6LJ+ainWe0JxxyDHi8i2SWr8QX00guvIfFrsHQ==" });
        }
    }
}
