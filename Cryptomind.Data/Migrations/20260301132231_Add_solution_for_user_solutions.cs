using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cryptomind.Data.Migrations
{
	/// <inheritdoc />
	public partial class Add_solution_for_user_solutions : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<string>(
				name: "Solution",
				table: "UserSolution",
				type: "nvarchar(max)",
				nullable: false,
				defaultValue: "");

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

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				name: "Solution",
				table: "UserSolution");

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
		}
	}
}
