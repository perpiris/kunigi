using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kunigi.Data.Migrations
{
    /// <inheritdoc />
    public partial class teamupdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ParentGameMediaFiles_Games_GameId",
                table: "ParentGameMediaFiles");

            migrationBuilder.DropIndex(
                name: "IX_ParentGameMediaFiles_GameId",
                table: "ParentGameMediaFiles");

            migrationBuilder.DropColumn(
                name: "GameId",
                table: "ParentGameMediaFiles");

            migrationBuilder.AddColumn<int>(
                name: "CreatedYear",
                table: "Teams",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Teams",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedYear",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Teams");

            migrationBuilder.AddColumn<int>(
                name: "GameId",
                table: "ParentGameMediaFiles",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ParentGameMediaFiles_GameId",
                table: "ParentGameMediaFiles",
                column: "GameId");

            migrationBuilder.AddForeignKey(
                name: "FK_ParentGameMediaFiles_Games_GameId",
                table: "ParentGameMediaFiles",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "GameId");
        }
    }
}
