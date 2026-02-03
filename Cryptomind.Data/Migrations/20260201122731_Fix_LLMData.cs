using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cryptomind.Data.Migrations
{
    /// <inheritdoc />
    public partial class Fix_LLMData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CachedTypeHint",
                table: "Cipher",
                newName: "LLMData_CachedTypeHint");

            migrationBuilder.RenameColumn(
                name: "CachedSolution",
                table: "Cipher",
                newName: "LLMData_CachedSolution");

            migrationBuilder.RenameColumn(
                name: "CachedHint",
                table: "Cipher",
                newName: "LLMData_CachedHint");

            migrationBuilder.RenameColumn(
                name: "LLMReasoning",
                table: "Cipher",
                newName: "LLMData_Reasoning");

            migrationBuilder.RenameColumn(
                name: "LLMIssues",
                table: "Cipher",
                newName: "LLMData_Issues");

            migrationBuilder.RenameColumn(
                name: "LLMConfidence",
                table: "Cipher",
                newName: "LLMData_Confidence");

            migrationBuilder.RenameColumn(
                name: "LLMAnalysis",
                table: "Cipher",
                newName: "LLMData_Analysis");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LLMData_CachedTypeHint",
                table: "Cipher",
                newName: "CachedTypeHint");

            migrationBuilder.RenameColumn(
                name: "LLMData_CachedSolution",
                table: "Cipher",
                newName: "CachedSolution");

            migrationBuilder.RenameColumn(
                name: "LLMData_CachedHint",
                table: "Cipher",
                newName: "CachedHint");

            migrationBuilder.RenameColumn(
                name: "LLMData_Reasoning",
                table: "Cipher",
                newName: "LLMReasoning");

            migrationBuilder.RenameColumn(
                name: "LLMData_Issues",
                table: "Cipher",
                newName: "LLMIssues");

            migrationBuilder.RenameColumn(
                name: "LLMData_Confidence",
                table: "Cipher",
                newName: "LLMConfidence");

            migrationBuilder.RenameColumn(
                name: "LLMData_Analysis",
                table: "Cipher",
                newName: "LLMAnalysis");
        }
    }
}
