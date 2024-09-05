using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kunigi.Data.Migrations
{
    /// <inheritdoc />
    public partial class idupdates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GameYearMediaFiles_Games_GameId",
                table: "GameYearMediaFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_GameYearMediaFiles_MediaFiles_MediaFileId",
                table: "GameYearMediaFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_GameYearMediaFiles_ParentGames_ParentGameId",
                table: "GameYearMediaFiles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GameYearMediaFiles",
                table: "GameYearMediaFiles");

            migrationBuilder.RenameTable(
                name: "GameYearMediaFiles",
                newName: "ParentGameMediaFiles");

            migrationBuilder.RenameColumn(
                name: "TeamFolderUrl",
                table: "Teams",
                newName: "TeamFolderPath");

            migrationBuilder.RenameColumn(
                name: "ProfileImageUrl",
                table: "Teams",
                newName: "TeamProfileImagePath");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Puzzles",
                newName: "PuzzleId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "PuzzleMediaFiles",
                newName: "PuzzleMediaId");

            migrationBuilder.RenameColumn(
                name: "ProfileImageUrl",
                table: "ParentGames",
                newName: "ParentGameProfileImagePath");

            migrationBuilder.RenameColumn(
                name: "ParentGameFolderUrl",
                table: "ParentGames",
                newName: "ParentGameFolderPath");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "ParentGames",
                newName: "ParentGameId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "MediaFiles",
                newName: "MediaFileId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "GameTypes",
                newName: "GameTypeId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Games",
                newName: "GameId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "ParentGameMediaFiles",
                newName: "ParentGameMediaId");

            migrationBuilder.RenameIndex(
                name: "IX_GameYearMediaFiles_ParentGameId_MediaFileId",
                table: "ParentGameMediaFiles",
                newName: "IX_ParentGameMediaFiles_ParentGameId_MediaFileId");

            migrationBuilder.RenameIndex(
                name: "IX_GameYearMediaFiles_MediaFileId",
                table: "ParentGameMediaFiles",
                newName: "IX_ParentGameMediaFiles_MediaFileId");

            migrationBuilder.RenameIndex(
                name: "IX_GameYearMediaFiles_GameId",
                table: "ParentGameMediaFiles",
                newName: "IX_ParentGameMediaFiles_GameId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ParentGameMediaFiles",
                table: "ParentGameMediaFiles",
                column: "ParentGameMediaId");

            migrationBuilder.AddForeignKey(
                name: "FK_ParentGameMediaFiles_Games_GameId",
                table: "ParentGameMediaFiles",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "GameId");

            migrationBuilder.AddForeignKey(
                name: "FK_ParentGameMediaFiles_MediaFiles_MediaFileId",
                table: "ParentGameMediaFiles",
                column: "MediaFileId",
                principalTable: "MediaFiles",
                principalColumn: "MediaFileId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ParentGameMediaFiles_ParentGames_ParentGameId",
                table: "ParentGameMediaFiles",
                column: "ParentGameId",
                principalTable: "ParentGames",
                principalColumn: "ParentGameId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ParentGameMediaFiles_Games_GameId",
                table: "ParentGameMediaFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_ParentGameMediaFiles_MediaFiles_MediaFileId",
                table: "ParentGameMediaFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_ParentGameMediaFiles_ParentGames_ParentGameId",
                table: "ParentGameMediaFiles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ParentGameMediaFiles",
                table: "ParentGameMediaFiles");

            migrationBuilder.RenameTable(
                name: "ParentGameMediaFiles",
                newName: "GameYearMediaFiles");

            migrationBuilder.RenameColumn(
                name: "TeamProfileImagePath",
                table: "Teams",
                newName: "ProfileImageUrl");

            migrationBuilder.RenameColumn(
                name: "TeamFolderPath",
                table: "Teams",
                newName: "TeamFolderUrl");

            migrationBuilder.RenameColumn(
                name: "PuzzleId",
                table: "Puzzles",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "PuzzleMediaId",
                table: "PuzzleMediaFiles",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "ParentGameProfileImagePath",
                table: "ParentGames",
                newName: "ProfileImageUrl");

            migrationBuilder.RenameColumn(
                name: "ParentGameFolderPath",
                table: "ParentGames",
                newName: "ParentGameFolderUrl");

            migrationBuilder.RenameColumn(
                name: "ParentGameId",
                table: "ParentGames",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "MediaFileId",
                table: "MediaFiles",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "GameTypeId",
                table: "GameTypes",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "GameId",
                table: "Games",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "ParentGameMediaId",
                table: "GameYearMediaFiles",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_ParentGameMediaFiles_ParentGameId_MediaFileId",
                table: "GameYearMediaFiles",
                newName: "IX_GameYearMediaFiles_ParentGameId_MediaFileId");

            migrationBuilder.RenameIndex(
                name: "IX_ParentGameMediaFiles_MediaFileId",
                table: "GameYearMediaFiles",
                newName: "IX_GameYearMediaFiles_MediaFileId");

            migrationBuilder.RenameIndex(
                name: "IX_ParentGameMediaFiles_GameId",
                table: "GameYearMediaFiles",
                newName: "IX_GameYearMediaFiles_GameId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GameYearMediaFiles",
                table: "GameYearMediaFiles",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GameYearMediaFiles_Games_GameId",
                table: "GameYearMediaFiles",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GameYearMediaFiles_MediaFiles_MediaFileId",
                table: "GameYearMediaFiles",
                column: "MediaFileId",
                principalTable: "MediaFiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GameYearMediaFiles_ParentGames_ParentGameId",
                table: "GameYearMediaFiles",
                column: "ParentGameId",
                principalTable: "ParentGames",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
