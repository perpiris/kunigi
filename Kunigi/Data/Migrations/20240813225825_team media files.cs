using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kunigi.Data.Migrations
{
    /// <inheritdoc />
    public partial class teammediafiles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MediaFiles_Games",
                table: "MediaFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_MediaFiles_Teams",
                table: "MediaFiles");

            migrationBuilder.DropIndex(
                name: "IX_MediaFiles_ParentId_ParentType",
                table: "MediaFiles");

            migrationBuilder.AddColumn<int>(
                name: "GameId",
                table: "MediaFiles",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TeamMediaFiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TeamId = table.Column<int>(type: "INTEGER", nullable: false),
                    MediaFileId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamMediaFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeamMediaFiles_MediaFiles_MediaFileId",
                        column: x => x.MediaFileId,
                        principalTable: "MediaFiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TeamMediaFiles_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MediaFiles_GameId",
                table: "MediaFiles",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamMediaFiles_MediaFileId",
                table: "TeamMediaFiles",
                column: "MediaFileId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamMediaFiles_TeamId_MediaFileId",
                table: "TeamMediaFiles",
                columns: new[] { "TeamId", "MediaFileId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_MediaFiles_Games_GameId",
                table: "MediaFiles",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MediaFiles_Games_GameId",
                table: "MediaFiles");

            migrationBuilder.DropTable(
                name: "TeamMediaFiles");

            migrationBuilder.DropIndex(
                name: "IX_MediaFiles_GameId",
                table: "MediaFiles");

            migrationBuilder.DropColumn(
                name: "GameId",
                table: "MediaFiles");

            migrationBuilder.CreateIndex(
                name: "IX_MediaFiles_ParentId_ParentType",
                table: "MediaFiles",
                columns: new[] { "ParentId", "ParentType" });

            migrationBuilder.AddForeignKey(
                name: "FK_MediaFiles_Games",
                table: "MediaFiles",
                column: "ParentId",
                principalTable: "Games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MediaFiles_Teams",
                table: "MediaFiles",
                column: "ParentId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
