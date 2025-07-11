using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cryptomind.Data.Migrations
{
    /// <inheritdoc />
    public partial class Added_UserSolution_Id : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserSolution",
                table: "UserSolution");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "UserSolution",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserSolution",
                table: "UserSolution",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_UserSolution_CipherId",
                table: "UserSolution",
                column: "CipherId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserSolution",
                table: "UserSolution");

            migrationBuilder.DropIndex(
                name: "IX_UserSolution_CipherId",
                table: "UserSolution");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "UserSolution");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserSolution",
                table: "UserSolution",
                columns: new[] { "CipherId", "UserId" });
        }
    }
}
