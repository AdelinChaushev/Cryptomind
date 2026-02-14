using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cryptomind.Data.Migrations
{
    /// <inheritdoc />
    public partial class Cipher_LLMUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LLMData_Analysis",
                table: "Cipher",
                newName: "LLMData_PredictedType");

            migrationBuilder.RenameColumn(
                name: "isBanned",
                table: "AspNetUsers",
                newName: "IsBanned");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LLMData_PredictedType",
                table: "Cipher",
                newName: "LLMData_Analysis");

            migrationBuilder.RenameColumn(
                name: "IsBanned",
                table: "AspNetUsers",
                newName: "isBanned");
        }
    }
}
