using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kunigi.Data.Migrations
{
    /// <inheritdoc />
    public partial class slug : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Games_GameYears_GameYearId",
                table: "Games");

            migrationBuilder.DropForeignKey(
                name: "FK_MediaFiles_GameYears_GameYearId",
                table: "MediaFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_MediaFiles_Puzzles_PuzzleId",
                table: "MediaFiles");

            migrationBuilder.DropIndex(
                name: "IX_MediaFiles_GameYearId",
                table: "MediaFiles");

            migrationBuilder.DropIndex(
                name: "IX_MediaFiles_PuzzleId",
                table: "MediaFiles");

            migrationBuilder.DropIndex(
                name: "IX_Games_GameYearId",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "GameYearId",
                table: "MediaFiles");

            migrationBuilder.DropColumn(
                name: "PuzzleId",
                table: "MediaFiles");

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "GameTypes",
                type: "varchar(255)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ParentGameId",
                table: "Games",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "GameYearMediaFiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GameYearId = table.Column<int>(type: "INTEGER", nullable: false),
                    MediaFileId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameYearMediaFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameYearMediaFiles_GameYears_GameYearId",
                        column: x => x.GameYearId,
                        principalTable: "GameYears",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GameYearMediaFiles_MediaFiles_MediaFileId",
                        column: x => x.MediaFileId,
                        principalTable: "MediaFiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PuzzleMediaFiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PuzzleId = table.Column<int>(type: "INTEGER", nullable: false),
                    MediaFileId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PuzzleMediaFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PuzzleMediaFiles_MediaFiles_MediaFileId",
                        column: x => x.MediaFileId,
                        principalTable: "MediaFiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PuzzleMediaFiles_Puzzles_PuzzleId",
                        column: x => x.PuzzleId,
                        principalTable: "Puzzles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Games_ParentGameId",
                table: "Games",
                column: "ParentGameId");

            migrationBuilder.CreateIndex(
                name: "IX_GameYearMediaFiles_GameYearId_MediaFileId",
                table: "GameYearMediaFiles",
                columns: new[] { "GameYearId", "MediaFileId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GameYearMediaFiles_MediaFileId",
                table: "GameYearMediaFiles",
                column: "MediaFileId");

            migrationBuilder.CreateIndex(
                name: "IX_PuzzleMediaFiles_MediaFileId",
                table: "PuzzleMediaFiles",
                column: "MediaFileId");

            migrationBuilder.CreateIndex(
                name: "IX_PuzzleMediaFiles_PuzzleId_MediaFileId",
                table: "PuzzleMediaFiles",
                columns: new[] { "PuzzleId", "MediaFileId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Games_GameYears_ParentGameId",
                table: "Games",
                column: "ParentGameId",
                principalTable: "GameYears",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Games_GameYears_ParentGameId",
                table: "Games");

            migrationBuilder.DropTable(
                name: "GameYearMediaFiles");

            migrationBuilder.DropTable(
                name: "PuzzleMediaFiles");

            migrationBuilder.DropIndex(
                name: "IX_Games_ParentGameId",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "GameTypes");

            migrationBuilder.DropColumn(
                name: "ParentGameId",
                table: "Games");

            migrationBuilder.AddColumn<int>(
                name: "GameYearId",
                table: "MediaFiles",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PuzzleId",
                table: "MediaFiles",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MediaFiles_GameYearId",
                table: "MediaFiles",
                column: "GameYearId");

            migrationBuilder.CreateIndex(
                name: "IX_MediaFiles_PuzzleId",
                table: "MediaFiles",
                column: "PuzzleId");

            migrationBuilder.CreateIndex(
                name: "IX_Games_GameYearId",
                table: "Games",
                column: "GameYearId");

            migrationBuilder.AddForeignKey(
                name: "FK_Games_GameYears_GameYearId",
                table: "Games",
                column: "GameYearId",
                principalTable: "GameYears",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MediaFiles_GameYears_GameYearId",
                table: "MediaFiles",
                column: "GameYearId",
                principalTable: "GameYears",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MediaFiles_Puzzles_PuzzleId",
                table: "MediaFiles",
                column: "PuzzleId",
                principalTable: "Puzzles",
                principalColumn: "Id");
        }
    }
}
