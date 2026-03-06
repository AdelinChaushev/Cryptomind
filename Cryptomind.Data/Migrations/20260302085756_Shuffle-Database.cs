using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cryptomind.Data.Migrations
{
    /// <inheritdoc />
    public partial class ShuffleDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "1c4c787f-7864-4ea0-9b9f-a6a351da7cf0", "AQAAAAIAAYagAAAAECy+bBs48C27VzQa72zNi3E1NzzJ0jT6UPyF9BMqaPmTIz5GlQL39lFWLHitHW69Sg==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b2c3d4e5-f6a7-8901-bcde-f12345678901",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "ed2dc99d-f765-4907-b30b-74a7ee29f6fc", "AQAAAAIAAYagAAAAEPZVwCwgjVHmq+s6k6ej5gSxsnK64xaUHG9MtO6j6DPSm6htC4jSyrKarvxmmjgm7w==" });
        }
    }
}
