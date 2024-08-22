using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kunigi.Data.Migrations
{
    /// <inheritdoc />
    public partial class asdasdasdasdadasdasd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Games_ParentGames_ParentGameId",
                table: "Games");

            migrationBuilder.DropForeignKey(
                name: "FK_GameYearMediaFiles_ParentGames_GameYearId",
                table: "GameYearMediaFiles");

            migrationBuilder.DropColumn(
                name: "GameYearId",
                table: "Games");

            migrationBuilder.RenameColumn(
                name: "GameYearId",
                table: "GameYearMediaFiles",
                newName: "ParentGameId");

            migrationBuilder.RenameIndex(
                name: "IX_GameYearMediaFiles_GameYearId_MediaFileId",
                table: "GameYearMediaFiles",
                newName: "IX_GameYearMediaFiles_ParentGameId_MediaFileId");

            migrationBuilder.AlterColumn<int>(
                name: "ParentGameId",
                table: "Games",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Games_ParentGames_ParentGameId",
                table: "Games",
                column: "ParentGameId",
                principalTable: "ParentGames",
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Games_ParentGames_ParentGameId",
                table: "Games");

            migrationBuilder.DropForeignKey(
                name: "FK_GameYearMediaFiles_ParentGames_ParentGameId",
                table: "GameYearMediaFiles");

            migrationBuilder.RenameColumn(
                name: "ParentGameId",
                table: "GameYearMediaFiles",
                newName: "GameYearId");

            migrationBuilder.RenameIndex(
                name: "IX_GameYearMediaFiles_ParentGameId_MediaFileId",
                table: "GameYearMediaFiles",
                newName: "IX_GameYearMediaFiles_GameYearId_MediaFileId");

            migrationBuilder.AlterColumn<int>(
                name: "ParentGameId",
                table: "Games",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<int>(
                name: "GameYearId",
                table: "Games",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_Games_ParentGames_ParentGameId",
                table: "Games",
                column: "ParentGameId",
                principalTable: "ParentGames",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GameYearMediaFiles_ParentGames_GameYearId",
                table: "GameYearMediaFiles",
                column: "GameYearId",
                principalTable: "ParentGames",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
