using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cryptomind.Data.Migrations
{
    /// <inheritdoc />
    public partial class Additional_LLMData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "LLMData_IsAppropriate",
                table: "Cipher",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "LLMData_IsSolvable",
                table: "Cipher",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "LLMData_SolutionCorrect",
                table: "Cipher",
                type: "bit",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LLMData_IsAppropriate",
                table: "Cipher");

            migrationBuilder.DropColumn(
                name: "LLMData_IsSolvable",
                table: "Cipher");

            migrationBuilder.DropColumn(
                name: "LLMData_SolutionCorrect",
                table: "Cipher");
        }
    }
}
