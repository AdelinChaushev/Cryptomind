using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cryptomind.Data.Migrations
{
	/// <inheritdoc />
	public partial class Fixnaming : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.RenameColumn(
				name: "UplodaedTime",
				table: "AnswerSuggestions",
				newName: "UploadedTime");

			migrationBuilder.UpdateData(
				table: "AspNetUsers",
				keyColumn: "Id",
				keyValue: "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
				columns: new[] { "ConcurrencyStamp", "PasswordHash" },
				values: new object[] { "138df409-428e-495d-8f80-6802d6604618", "AQAAAAIAAYagAAAAELQU/H9FBYfccBKf+VoQ04UWAn0sZyBdkFo7G9prxZhbgC3fRgm8qd3oxq4LiwTvzw==" });

			migrationBuilder.UpdateData(
				table: "AspNetUsers",
				keyColumn: "Id",
				keyValue: "b2c3d4e5-f6a7-8901-bcde-f12345678901",
				columns: new[] { "ConcurrencyStamp", "PasswordHash" },
				values: new object[] { "9f8f0f10-1b64-4f2a-822f-638f65f18221", "AQAAAAIAAYagAAAAEF3mwZoDXYvOoKr4bQrxTCaVPbfYD4XfmuTO1J3AwJRo+KYWedkpcXWjwOzkupNMFg==" });
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.RenameColumn(
				name: "UploadedTime",
				table: "AnswerSuggestions",
				newName: "UplodaedTime");

			migrationBuilder.UpdateData(
				table: "AspNetUsers",
				keyColumn: "Id",
				keyValue: "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
				columns: new[] { "ConcurrencyStamp", "PasswordHash" },
				values: new object[] { "a6de731b-2258-4773-998f-115edcc37a43", "AQAAAAIAAYagAAAAEDljVwiXOjQ1i3lObm+Q6gepHa/3pY5LjM9c8pnfw7a3/EKKPj7LKr8hfKbcDfaxaQ==" });

			migrationBuilder.UpdateData(
				table: "AspNetUsers",
				keyColumn: "Id",
				keyValue: "b2c3d4e5-f6a7-8901-bcde-f12345678901",
				columns: new[] { "ConcurrencyStamp", "PasswordHash" },
				values: new object[] { "8796aa41-091d-4d95-b256-e0e9d0b785bc", "AQAAAAIAAYagAAAAELLnHUhDH0eMIU6bXaQaC88GHO3MTRi8DhlCotEHX5gy5KEIgjzTS79/pWCR3bikMQ==" });
		}
	}
}
