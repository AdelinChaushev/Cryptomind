using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cryptomind.Data.Migrations
{
    /// <inheritdoc />
    public partial class fix_llldata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "a2e76b2e-c304-4ba4-867a-13f0b4a25c76", "AQAAAAIAAYagAAAAEPchmZoLAMWUfxz4NNIiQbFGoXXI75DCiIMtFdy2h3XaI2WiN/wjOEfd4G8vXZQIxg==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b2c3d4e5-f6a7-8901-bcde-f12345678901",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "be55590e-35b6-4d04-8883-25dfe4082a92", "AQAAAAIAAYagAAAAEEOqmYcw3jMJgfpG37TfT8QEU/w0PHo8ar4YhyGDCYXrFGUujFHcYGs2GQhGayOHpA==" });

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "LLMData_CachedHint", "LLMData_CachedSolution", "LLMData_CachedTypeHint", "LLMData_Confidence", "LLMData_IsAppropriate", "LLMData_IsSolvable", "LLMData_Issues", "LLMData_PredictedType", "LLMData_Reasoning", "LLMData_SolutionCorrect" },
                values: new object[] { "", "", "", "High", true, true, null, "Base64", null, true });

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "LLMData_CachedHint", "LLMData_CachedSolution", "LLMData_CachedTypeHint", "LLMData_Confidence", "LLMData_IsAppropriate", "LLMData_IsSolvable", "LLMData_Issues", "LLMData_PredictedType", "LLMData_Reasoning", "LLMData_SolutionCorrect" },
                values: new object[] { "", "", "", "High", true, true, null, "Base64", null, true });

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "LLMData_CachedHint", "LLMData_CachedSolution", "LLMData_CachedTypeHint", "LLMData_Confidence", "LLMData_IsAppropriate", "LLMData_IsSolvable", "LLMData_Issues", "LLMData_PredictedType", "LLMData_Reasoning", "LLMData_SolutionCorrect" },
                values: new object[] { "", "", "", "High", true, true, null, "Base64", null, true });

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "LLMData_CachedHint", "LLMData_CachedSolution", "LLMData_CachedTypeHint", "LLMData_Confidence", "LLMData_IsAppropriate", "LLMData_IsSolvable", "LLMData_Issues", "LLMData_PredictedType", "LLMData_Reasoning", "LLMData_SolutionCorrect" },
                values: new object[] { "", "", "", "High", true, true, null, "Hex", null, true });

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "LLMData_CachedHint", "LLMData_CachedSolution", "LLMData_CachedTypeHint", "LLMData_Confidence", "LLMData_IsAppropriate", "LLMData_IsSolvable", "LLMData_Issues", "LLMData_PredictedType", "LLMData_Reasoning", "LLMData_SolutionCorrect" },
                values: new object[] { "", "", "", "High", true, true, null, "Hex", null, true });

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "LLMData_CachedHint", "LLMData_CachedSolution", "LLMData_CachedTypeHint", "LLMData_Confidence", "LLMData_IsAppropriate", "LLMData_IsSolvable", "LLMData_Issues", "LLMData_PredictedType", "LLMData_Reasoning", "LLMData_SolutionCorrect" },
                values: new object[] { "", "", "", "High", true, true, null, "Hex", null, true });

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "LLMData_CachedHint", "LLMData_CachedSolution", "LLMData_CachedTypeHint", "LLMData_Confidence", "LLMData_IsAppropriate", "LLMData_IsSolvable", "LLMData_Issues", "LLMData_PredictedType", "LLMData_Reasoning", "LLMData_SolutionCorrect" },
                values: new object[] { "", "", "", "High", true, true, null, "Binary", null, true });

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "LLMData_CachedHint", "LLMData_CachedSolution", "LLMData_CachedTypeHint", "LLMData_Confidence", "LLMData_IsAppropriate", "LLMData_IsSolvable", "LLMData_Issues", "LLMData_PredictedType", "LLMData_Reasoning", "LLMData_SolutionCorrect" },
                values: new object[] { "", "", "", "High", true, true, null, "Binary", null, true });

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "LLMData_CachedHint", "LLMData_CachedSolution", "LLMData_CachedTypeHint", "LLMData_Confidence", "LLMData_IsAppropriate", "LLMData_IsSolvable", "LLMData_Issues", "LLMData_PredictedType", "LLMData_Reasoning", "LLMData_SolutionCorrect" },
                values: new object[] { "", "", "", "High", true, true, null, "Binary", null, true });

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "LLMData_CachedHint", "LLMData_CachedSolution", "LLMData_CachedTypeHint", "LLMData_Confidence", "LLMData_IsAppropriate", "LLMData_IsSolvable", "LLMData_Issues", "LLMData_PredictedType", "LLMData_Reasoning", "LLMData_SolutionCorrect" },
                values: new object[] { "", "", "", "High", true, true, null, "Morse", null, true });

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "LLMData_CachedHint", "LLMData_CachedSolution", "LLMData_CachedTypeHint", "LLMData_Confidence", "LLMData_IsAppropriate", "LLMData_IsSolvable", "LLMData_Issues", "LLMData_PredictedType", "LLMData_Reasoning", "LLMData_SolutionCorrect" },
                values: new object[] { "", "", "", "High", true, true, null, "Morse", null, true });

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "LLMData_CachedHint", "LLMData_CachedSolution", "LLMData_CachedTypeHint", "LLMData_Confidence", "LLMData_IsAppropriate", "LLMData_IsSolvable", "LLMData_Issues", "LLMData_PredictedType", "LLMData_Reasoning", "LLMData_SolutionCorrect" },
                values: new object[] { "", "", "", "High", true, true, null, "Morse", null, true });

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "LLMData_CachedHint", "LLMData_CachedSolution", "LLMData_CachedTypeHint", "LLMData_Confidence", "LLMData_IsAppropriate", "LLMData_IsSolvable", "LLMData_Issues", "LLMData_PredictedType", "LLMData_Reasoning", "LLMData_SolutionCorrect" },
                values: new object[] { "", "", "", "High", true, true, null, "ROT13", null, true });

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "LLMData_CachedHint", "LLMData_CachedSolution", "LLMData_CachedTypeHint", "LLMData_Confidence", "LLMData_IsAppropriate", "LLMData_IsSolvable", "LLMData_Issues", "LLMData_PredictedType", "LLMData_Reasoning", "LLMData_SolutionCorrect" },
                values: new object[] { "", "", "", "High", true, true, null, "ROT13", null, true });

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "LLMData_CachedHint", "LLMData_CachedSolution", "LLMData_CachedTypeHint", "LLMData_Confidence", "LLMData_IsAppropriate", "LLMData_IsSolvable", "LLMData_Issues", "LLMData_PredictedType", "LLMData_Reasoning", "LLMData_SolutionCorrect" },
                values: new object[] { "", "", "", "High", true, true, null, "ROT13", null, true });

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "LLMData_CachedHint", "LLMData_CachedSolution", "LLMData_CachedTypeHint", "LLMData_Confidence", "LLMData_IsAppropriate", "LLMData_IsSolvable", "LLMData_Issues", "LLMData_PredictedType", "LLMData_Reasoning", "LLMData_SolutionCorrect" },
                values: new object[] { "", "", "", "High", true, true, null, "Caesar", null, true });

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "LLMData_CachedHint", "LLMData_CachedSolution", "LLMData_CachedTypeHint", "LLMData_Confidence", "LLMData_IsAppropriate", "LLMData_IsSolvable", "LLMData_Issues", "LLMData_PredictedType", "LLMData_Reasoning", "LLMData_SolutionCorrect" },
                values: new object[] { "", "", "", "High", true, true, null, "Caesar", null, true });

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "LLMData_CachedHint", "LLMData_CachedSolution", "LLMData_CachedTypeHint", "LLMData_Confidence", "LLMData_IsAppropriate", "LLMData_IsSolvable", "LLMData_Issues", "LLMData_PredictedType", "LLMData_Reasoning", "LLMData_SolutionCorrect" },
                values: new object[] { "", "", "", "High", true, true, null, "Caesar", null, true });

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 19,
                columns: new[] { "LLMData_CachedHint", "LLMData_CachedSolution", "LLMData_CachedTypeHint", "LLMData_Confidence", "LLMData_IsAppropriate", "LLMData_IsSolvable", "LLMData_Issues", "LLMData_PredictedType", "LLMData_Reasoning", "LLMData_SolutionCorrect" },
                values: new object[] { "", "", "", "High", true, true, null, "Caesar", null, true });

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "LLMData_CachedHint", "LLMData_CachedSolution", "LLMData_CachedTypeHint", "LLMData_Confidence", "LLMData_IsAppropriate", "LLMData_IsSolvable", "LLMData_Issues", "LLMData_PredictedType", "LLMData_Reasoning", "LLMData_SolutionCorrect" },
                values: new object[] { "", "", "", "High", true, true, null, "Caesar", null, true });

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 21,
                columns: new[] { "LLMData_CachedHint", "LLMData_CachedSolution", "LLMData_CachedTypeHint", "LLMData_Confidence", "LLMData_IsAppropriate", "LLMData_IsSolvable", "LLMData_Issues", "LLMData_PredictedType", "LLMData_Reasoning", "LLMData_SolutionCorrect" },
                values: new object[] { "", "", "", "High", true, true, null, "Atbash", null, true });

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 22,
                columns: new[] { "LLMData_CachedHint", "LLMData_CachedSolution", "LLMData_CachedTypeHint", "LLMData_Confidence", "LLMData_IsAppropriate", "LLMData_IsSolvable", "LLMData_Issues", "LLMData_PredictedType", "LLMData_Reasoning", "LLMData_SolutionCorrect" },
                values: new object[] { "", "", "", "High", true, true, null, "Atbash", null, true });

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 23,
                columns: new[] { "LLMData_CachedHint", "LLMData_CachedSolution", "LLMData_CachedTypeHint", "LLMData_Confidence", "LLMData_IsAppropriate", "LLMData_IsSolvable", "LLMData_Issues", "LLMData_PredictedType", "LLMData_Reasoning", "LLMData_SolutionCorrect" },
                values: new object[] { "", "", "", "High", true, true, null, "Atbash", null, true });

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 24,
                columns: new[] { "LLMData_CachedHint", "LLMData_CachedSolution", "LLMData_CachedTypeHint", "LLMData_Confidence", "LLMData_IsAppropriate", "LLMData_IsSolvable", "LLMData_Issues", "LLMData_PredictedType", "LLMData_Reasoning", "LLMData_SolutionCorrect" },
                values: new object[] { "", "", "", "High", true, true, null, "SimpleSubstitution", null, true });

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 25,
                columns: new[] { "LLMData_CachedHint", "LLMData_CachedSolution", "LLMData_CachedTypeHint", "LLMData_Confidence", "LLMData_IsAppropriate", "LLMData_IsSolvable", "LLMData_Issues", "LLMData_PredictedType", "LLMData_Reasoning", "LLMData_SolutionCorrect" },
                values: new object[] { "", "", "", "High", true, true, null, "SimpleSubstitution", null, true });

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 26,
                columns: new[] { "LLMData_CachedHint", "LLMData_CachedSolution", "LLMData_CachedTypeHint", "LLMData_Confidence", "LLMData_IsAppropriate", "LLMData_IsSolvable", "LLMData_Issues", "LLMData_PredictedType", "LLMData_Reasoning", "LLMData_SolutionCorrect" },
                values: new object[] { "", "", "", "High", true, true, null, "SimpleSubstitution", null, true });

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 27,
                columns: new[] { "LLMData_CachedHint", "LLMData_CachedSolution", "LLMData_CachedTypeHint", "LLMData_Confidence", "LLMData_IsAppropriate", "LLMData_IsSolvable", "LLMData_Issues", "LLMData_PredictedType", "LLMData_Reasoning", "LLMData_SolutionCorrect" },
                values: new object[] { "", "", "", "High", true, true, null, "SimpleSubstitution", null, true });

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 28,
                columns: new[] { "LLMData_CachedHint", "LLMData_CachedSolution", "LLMData_CachedTypeHint", "LLMData_Confidence", "LLMData_IsAppropriate", "LLMData_IsSolvable", "LLMData_Issues", "LLMData_PredictedType", "LLMData_Reasoning", "LLMData_SolutionCorrect" },
                values: new object[] { "", "", "", "High", true, true, null, "RailFence", null, true });

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 29,
                columns: new[] { "LLMData_CachedHint", "LLMData_CachedSolution", "LLMData_CachedTypeHint", "LLMData_Confidence", "LLMData_IsAppropriate", "LLMData_IsSolvable", "LLMData_Issues", "LLMData_PredictedType", "LLMData_Reasoning", "LLMData_SolutionCorrect" },
                values: new object[] { "", "", "", "Medium", true, true, null, "RailFence", null, true });

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 30,
                columns: new[] { "LLMData_CachedHint", "LLMData_CachedSolution", "LLMData_CachedTypeHint", "LLMData_Confidence", "LLMData_IsAppropriate", "LLMData_IsSolvable", "LLMData_Issues", "LLMData_PredictedType", "LLMData_Reasoning", "LLMData_SolutionCorrect" },
                values: new object[] { "", "", "", "Medium", true, true, null, "RailFence", null, true });

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 31,
                columns: new[] { "LLMData_CachedHint", "LLMData_CachedSolution", "LLMData_CachedTypeHint", "LLMData_Confidence", "LLMData_IsAppropriate", "LLMData_IsSolvable", "LLMData_Issues", "LLMData_PredictedType", "LLMData_Reasoning", "LLMData_SolutionCorrect" },
                values: new object[] { "", "", "", "High", true, true, null, "RailFence", null, true });

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 32,
                columns: new[] { "LLMData_CachedHint", "LLMData_CachedSolution", "LLMData_CachedTypeHint", "LLMData_Confidence", "LLMData_IsAppropriate", "LLMData_IsSolvable", "LLMData_Issues", "LLMData_PredictedType", "LLMData_Reasoning", "LLMData_SolutionCorrect" },
                values: new object[] { "", "", "", "Medium", true, true, null, "Trithemius", null, true });

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 33,
                columns: new[] { "LLMData_CachedHint", "LLMData_CachedSolution", "LLMData_CachedTypeHint", "LLMData_Confidence", "LLMData_IsAppropriate", "LLMData_IsSolvable", "LLMData_Issues", "LLMData_PredictedType", "LLMData_Reasoning", "LLMData_SolutionCorrect" },
                values: new object[] { "", "", "", "Medium", true, true, null, "Trithemius", null, true });

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 34,
                columns: new[] { "LLMData_CachedHint", "LLMData_CachedSolution", "LLMData_CachedTypeHint", "LLMData_Confidence", "LLMData_IsAppropriate", "LLMData_IsSolvable", "LLMData_Issues", "LLMData_PredictedType", "LLMData_Reasoning", "LLMData_SolutionCorrect" },
                values: new object[] { "", "", "", "Medium", true, true, null, "Trithemius", null, true });

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 35,
                columns: new[] { "LLMData_CachedHint", "LLMData_CachedSolution", "LLMData_CachedTypeHint", "LLMData_Confidence", "LLMData_IsAppropriate", "LLMData_IsSolvable", "LLMData_Issues", "LLMData_PredictedType", "LLMData_Reasoning", "LLMData_SolutionCorrect" },
                values: new object[] { "", "", "", "Medium", true, true, null, "Vigenere", null, true });

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 36,
                columns: new[] { "LLMData_CachedHint", "LLMData_CachedSolution", "LLMData_CachedTypeHint", "LLMData_Confidence", "LLMData_IsAppropriate", "LLMData_IsSolvable", "LLMData_Issues", "LLMData_PredictedType", "LLMData_Reasoning", "LLMData_SolutionCorrect" },
                values: new object[] { "", "", "", "Medium", true, true, null, "Vigenere", null, true });

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 37,
                columns: new[] { "LLMData_CachedHint", "LLMData_CachedSolution", "LLMData_CachedTypeHint", "LLMData_Confidence", "LLMData_IsAppropriate", "LLMData_IsSolvable", "LLMData_Issues", "LLMData_PredictedType", "LLMData_Reasoning", "LLMData_SolutionCorrect" },
                values: new object[] { "", "", "", "Medium", true, true, null, "Vigenere", null, true });

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 38,
                columns: new[] { "LLMData_CachedHint", "LLMData_CachedSolution", "LLMData_CachedTypeHint", "LLMData_Confidence", "LLMData_IsAppropriate", "LLMData_IsSolvable", "LLMData_Issues", "LLMData_PredictedType", "LLMData_Reasoning", "LLMData_SolutionCorrect" },
                values: new object[] { "", "", "", "Medium", true, true, null, "Vigenere", null, true });

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 39,
                columns: new[] { "LLMData_CachedHint", "LLMData_CachedSolution", "LLMData_CachedTypeHint", "LLMData_Confidence", "LLMData_IsAppropriate", "LLMData_IsSolvable", "LLMData_Issues", "LLMData_PredictedType", "LLMData_Reasoning", "LLMData_SolutionCorrect" },
                values: new object[] { "", "", "", "Medium", true, true, null, "Vigenere", null, true });

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 40,
                columns: new[] { "LLMData_CachedHint", "LLMData_CachedSolution", "LLMData_CachedTypeHint", "LLMData_Confidence", "LLMData_IsAppropriate", "LLMData_IsSolvable", "LLMData_Issues", "LLMData_PredictedType", "LLMData_Reasoning", "LLMData_SolutionCorrect" },
                values: new object[] { "", "", "", "Medium", true, true, null, "Columnar", null, true });

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 41,
                columns: new[] { "LLMData_CachedHint", "LLMData_CachedSolution", "LLMData_CachedTypeHint", "LLMData_Confidence", "LLMData_IsAppropriate", "LLMData_IsSolvable", "LLMData_Issues", "LLMData_PredictedType", "LLMData_Reasoning", "LLMData_SolutionCorrect" },
                values: new object[] { "", "", "", "Medium", true, true, null, "Columnar", null, true });

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 42,
                columns: new[] { "LLMData_CachedHint", "LLMData_CachedSolution", "LLMData_CachedTypeHint", "LLMData_Confidence", "LLMData_IsAppropriate", "LLMData_IsSolvable", "LLMData_Issues", "LLMData_PredictedType", "LLMData_Reasoning", "LLMData_SolutionCorrect" },
                values: new object[] { "", "", "", "Medium", true, true, null, "Columnar", null, true });

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 43,
                columns: new[] { "LLMData_CachedHint", "LLMData_CachedSolution", "LLMData_CachedTypeHint", "LLMData_Confidence", "LLMData_IsAppropriate", "LLMData_IsSolvable", "LLMData_Issues", "LLMData_PredictedType", "LLMData_Reasoning", "LLMData_SolutionCorrect" },
                values: new object[] { "", "", "", "Medium", true, true, null, "Columnar", null, true });

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 44,
                columns: new[] { "LLMData_CachedHint", "LLMData_CachedSolution", "LLMData_CachedTypeHint", "LLMData_Confidence", "LLMData_IsAppropriate", "LLMData_IsSolvable", "LLMData_Issues", "LLMData_PredictedType", "LLMData_Reasoning", "LLMData_SolutionCorrect" },
                values: new object[] { "", "", "", "Medium", true, true, null, "Route", null, true });

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 45,
                columns: new[] { "LLMData_CachedHint", "LLMData_CachedSolution", "LLMData_CachedTypeHint", "LLMData_Confidence", "LLMData_IsAppropriate", "LLMData_IsSolvable", "LLMData_Issues", "LLMData_PredictedType", "LLMData_Reasoning", "LLMData_SolutionCorrect" },
                values: new object[] { "", "", "", "Medium", true, true, null, "Route", null, true });

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 46,
                columns: new[] { "LLMData_CachedHint", "LLMData_CachedSolution", "LLMData_CachedTypeHint", "LLMData_Confidence", "LLMData_IsAppropriate", "LLMData_IsSolvable", "LLMData_Issues", "LLMData_PredictedType", "LLMData_Reasoning", "LLMData_SolutionCorrect" },
                values: new object[] { "", "", "", "Medium", true, true, null, "Route", null, true });

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 47,
                columns: new[] { "LLMData_CachedHint", "LLMData_CachedSolution", "LLMData_CachedTypeHint", "LLMData_Confidence", "LLMData_IsAppropriate", "LLMData_IsSolvable", "LLMData_Issues", "LLMData_PredictedType", "LLMData_Reasoning", "LLMData_SolutionCorrect" },
                values: new object[] { "", "", "", "Medium", true, true, null, "Autokey", null, true });

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 48,
                columns: new[] { "LLMData_CachedHint", "LLMData_CachedSolution", "LLMData_CachedTypeHint", "LLMData_Confidence", "LLMData_IsAppropriate", "LLMData_IsSolvable", "LLMData_Issues", "LLMData_PredictedType", "LLMData_Reasoning", "LLMData_SolutionCorrect" },
                values: new object[] { "", "", "", "Medium", true, true, null, "Autokey", null, true });

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 49,
                columns: new[] { "LLMData_CachedHint", "LLMData_CachedSolution", "LLMData_CachedTypeHint", "LLMData_Confidence", "LLMData_IsAppropriate", "LLMData_IsSolvable", "LLMData_Issues", "LLMData_PredictedType", "LLMData_Reasoning", "LLMData_SolutionCorrect" },
                values: new object[] { "", "", "", "Medium", true, true, null, "Autokey", null, true });

            migrationBuilder.UpdateData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 50,
                columns: new[] { "LLMData_CachedHint", "LLMData_CachedSolution", "LLMData_CachedTypeHint", "LLMData_Confidence", "LLMData_IsAppropriate", "LLMData_IsSolvable", "LLMData_Issues", "LLMData_PredictedType", "LLMData_Reasoning", "LLMData_SolutionCorrect" },
                values: new object[] { "", "", "", "Medium", true, true, null, "Autokey", null, true });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "cfb253e0-abf1-4afb-9cca-10b2e1089638", "AQAAAAIAAYagAAAAENXkDwhSp6K2s7Nijjph8Vr+5QzRF3SAm2MLDO6SFroVNlnCshL/Wm1A5sDg4c2Ftg==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b2c3d4e5-f6a7-8901-bcde-f12345678901",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "6534036f-c744-4fc8-9fe2-c2eb933acd26", "AQAAAAIAAYagAAAAEMLHQUHLCew7oD8lXcE9FdnyYQy5YPfCq2rzkXJ3AoEuETt8eMu0yETY5h1S0Ia45w==" });
        }
    }
}
