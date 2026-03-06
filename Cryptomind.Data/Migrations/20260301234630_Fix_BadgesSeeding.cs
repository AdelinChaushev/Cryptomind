using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cryptomind.Data.Migrations
{
	/// <inheritdoc />
	public partial class Fix_BadgesSeeding : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.UpdateData(
				table: "AspNetUsers",
				keyColumn: "Id",
				keyValue: "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
				columns: new[] { "ConcurrencyStamp", "PasswordHash" },
				values: new object[] { "5044362d-ce67-44cd-bc40-1e6b30b9a0fd", "AQAAAAIAAYagAAAAEMZ+7v93fEvx+Yff+P0KZfqp+PKZwStJb0/r8yjwtKwAqvL7SCSkc/L6ihbmJLvvfQ==" });

			migrationBuilder.UpdateData(
				table: "AspNetUsers",
				keyColumn: "Id",
				keyValue: "b2c3d4e5-f6a7-8901-bcde-f12345678901",
				columns: new[] { "ConcurrencyStamp", "PasswordHash" },
				values: new object[] { "eb0b6baf-4f24-4f0e-8dd7-6071f689b432", "AQAAAAIAAYagAAAAEMez0YOFr7T2+WFsnrJTatPXMKswKlsUZcLdYGgnSZZf0iO/eH5gg+tYRVkJhDUWuw==" });

			migrationBuilder.UpdateData(
				table: "Badges",
				keyColumn: "Id",
				keyValue: 12,
				column: "Category",
				value: 3);
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
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

			migrationBuilder.UpdateData(
				table: "Badges",
				keyColumn: "Id",
				keyValue: 12,
				column: "Category",
				value: 0);
		}
	}
}
