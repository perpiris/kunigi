using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kunigi.Data.Migrations
{
    /// <inheritdoc />
    public partial class asdasd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Games_GameYears_ParentGameId",
                table: "Games");

            migrationBuilder.DropForeignKey(
                name: "FK_GameYearMediaFiles_GameYears_GameYearId",
                table: "GameYearMediaFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_GameYears_Teams_HostId",
                table: "GameYears");

            migrationBuilder.DropForeignKey(
                name: "FK_GameYears_Teams_WinnerId",
                table: "GameYears");

            migrationBuilder.DropForeignKey(
                name: "FK_MediaFiles_Games_GameId",
                table: "MediaFiles");

            migrationBuilder.DropIndex(
                name: "IX_MediaFiles_GameId",
                table: "MediaFiles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GameYears",
                table: "GameYears");

            migrationBuilder.DropColumn(
                name: "GameId",
                table: "MediaFiles");

            migrationBuilder.RenameTable(
                name: "GameYears",
                newName: "ParentGames");

            migrationBuilder.RenameIndex(
                name: "IX_GameYears_WinnerId",
                table: "ParentGames",
                newName: "IX_ParentGames_WinnerId");

            migrationBuilder.RenameIndex(
                name: "IX_GameYears_HostId",
                table: "ParentGames",
                newName: "IX_ParentGames_HostId");

            migrationBuilder.AddColumn<int>(
                name: "GameId",
                table: "GameYearMediaFiles",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ParentGames",
                table: "ParentGames",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_GameYearMediaFiles_GameId",
                table: "GameYearMediaFiles",
                column: "GameId");

            migrationBuilder.AddForeignKey(
                name: "FK_Games_ParentGames_ParentGameId",
                table: "Games",
                column: "ParentGameId",
                principalTable: "ParentGames",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GameYearMediaFiles_Games_GameId",
                table: "GameYearMediaFiles",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GameYearMediaFiles_ParentGames_GameYearId",
                table: "GameYearMediaFiles",
                column: "GameYearId",
                principalTable: "ParentGames",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ParentGames_Teams_HostId",
                table: "ParentGames",
                column: "HostId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ParentGames_Teams_WinnerId",
                table: "ParentGames",
                column: "WinnerId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Games_ParentGames_ParentGameId",
                table: "Games");

            migrationBuilder.DropForeignKey(
                name: "FK_GameYearMediaFiles_Games_GameId",
                table: "GameYearMediaFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_GameYearMediaFiles_ParentGames_GameYearId",
                table: "GameYearMediaFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_ParentGames_Teams_HostId",
                table: "ParentGames");

            migrationBuilder.DropForeignKey(
                name: "FK_ParentGames_Teams_WinnerId",
                table: "ParentGames");

            migrationBuilder.DropIndex(
                name: "IX_GameYearMediaFiles_GameId",
                table: "GameYearMediaFiles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ParentGames",
                table: "ParentGames");

            migrationBuilder.DropColumn(
                name: "GameId",
                table: "GameYearMediaFiles");

            migrationBuilder.RenameTable(
                name: "ParentGames",
                newName: "GameYears");

            migrationBuilder.RenameIndex(
                name: "IX_ParentGames_WinnerId",
                table: "GameYears",
                newName: "IX_GameYears_WinnerId");

            migrationBuilder.RenameIndex(
                name: "IX_ParentGames_HostId",
                table: "GameYears",
                newName: "IX_GameYears_HostId");

            migrationBuilder.AddColumn<int>(
                name: "GameId",
                table: "MediaFiles",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_GameYears",
                table: "GameYears",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_MediaFiles_GameId",
                table: "MediaFiles",
                column: "GameId");

            migrationBuilder.AddForeignKey(
                name: "FK_Games_GameYears_ParentGameId",
                table: "Games",
                column: "ParentGameId",
                principalTable: "GameYears",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GameYearMediaFiles_GameYears_GameYearId",
                table: "GameYearMediaFiles",
                column: "GameYearId",
                principalTable: "GameYears",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GameYears_Teams_HostId",
                table: "GameYears",
                column: "HostId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GameYears_Teams_WinnerId",
                table: "GameYears",
                column: "WinnerId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MediaFiles_Games_GameId",
                table: "MediaFiles",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "Id");
        }
    }
}
