using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kunigi.Data.Migrations
{
    /// <inheritdoc />
    public partial class basemediafiles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TeamManagers_AspNetUsers_UserId",
                table: "TeamManagers");

            migrationBuilder.DropIndex(
                name: "IX_TeamManagers_UserId",
                table: "TeamManagers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Files",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "TeamManagers");

            migrationBuilder.RenameTable(
                name: "Files",
                newName: "MediaFiles");

            migrationBuilder.AlterColumn<string>(
                name: "AppUserId",
                table: "TeamManagers",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<int>(
                name: "GameYearId",
                table: "MediaFiles",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ParentType",
                table: "MediaFiles",
                type: "varchar(50)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PuzzleId",
                table: "MediaFiles",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_MediaFiles",
                table: "MediaFiles",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_TeamManagers_AppUserId",
                table: "TeamManagers",
                column: "AppUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Puzzles_GameId",
                table: "Puzzles",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_MediaFiles_GameYearId",
                table: "MediaFiles",
                column: "GameYearId");

            migrationBuilder.CreateIndex(
                name: "IX_MediaFiles_ParentId_ParentType",
                table: "MediaFiles",
                columns: new[] { "ParentId", "ParentType" });

            migrationBuilder.CreateIndex(
                name: "IX_MediaFiles_PuzzleId",
                table: "MediaFiles",
                column: "PuzzleId");

            migrationBuilder.AddForeignKey(
                name: "FK_MediaFiles_GameYears_GameYearId",
                table: "MediaFiles",
                column: "GameYearId",
                principalTable: "GameYears",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MediaFiles_Games",
                table: "MediaFiles",
                column: "ParentId",
                principalTable: "Games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MediaFiles_Puzzles_PuzzleId",
                table: "MediaFiles",
                column: "PuzzleId",
                principalTable: "Puzzles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MediaFiles_Teams",
                table: "MediaFiles",
                column: "ParentId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Puzzles_Games_GameId",
                table: "Puzzles",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TeamManagers_AspNetUsers_AppUserId",
                table: "TeamManagers",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MediaFiles_GameYears_GameYearId",
                table: "MediaFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_MediaFiles_Games",
                table: "MediaFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_MediaFiles_Puzzles_PuzzleId",
                table: "MediaFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_MediaFiles_Teams",
                table: "MediaFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_Puzzles_Games_GameId",
                table: "Puzzles");

            migrationBuilder.DropForeignKey(
                name: "FK_TeamManagers_AspNetUsers_AppUserId",
                table: "TeamManagers");

            migrationBuilder.DropIndex(
                name: "IX_TeamManagers_AppUserId",
                table: "TeamManagers");

            migrationBuilder.DropIndex(
                name: "IX_Puzzles_GameId",
                table: "Puzzles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MediaFiles",
                table: "MediaFiles");

            migrationBuilder.DropIndex(
                name: "IX_MediaFiles_GameYearId",
                table: "MediaFiles");

            migrationBuilder.DropIndex(
                name: "IX_MediaFiles_ParentId_ParentType",
                table: "MediaFiles");

            migrationBuilder.DropIndex(
                name: "IX_MediaFiles_PuzzleId",
                table: "MediaFiles");

            migrationBuilder.DropColumn(
                name: "GameYearId",
                table: "MediaFiles");

            migrationBuilder.DropColumn(
                name: "ParentType",
                table: "MediaFiles");

            migrationBuilder.DropColumn(
                name: "PuzzleId",
                table: "MediaFiles");

            migrationBuilder.RenameTable(
                name: "MediaFiles",
                newName: "Files");

            migrationBuilder.AlterColumn<int>(
                name: "AppUserId",
                table: "TeamManagers",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "TeamManagers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Files",
                table: "Files",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_TeamManagers_UserId",
                table: "TeamManagers",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_TeamManagers_AspNetUsers_UserId",
                table: "TeamManagers",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
