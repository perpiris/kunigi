using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kunigi.Data.Migrations
{
    /// <inheritdoc />
    public partial class yearfix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Games_Year_YearId",
                table: "Games");

            migrationBuilder.DropTable(
                name: "Year");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Games");

            migrationBuilder.RenameColumn(
                name: "YearId",
                table: "Games",
                newName: "GameYearId");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Games",
                newName: "Category");

            migrationBuilder.RenameIndex(
                name: "IX_Games_YearId",
                table: "Games",
                newName: "IX_Games_GameYearId");

            migrationBuilder.CreateTable(
                name: "Files",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Path = table.Column<string>(type: "varchar(255)", nullable: true),
                    ParentId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Files", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GameYears",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Year = table.Column<short>(type: "INTEGER", nullable: false),
                    Title = table.Column<string>(type: "varchar(255)", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Slug = table.Column<string>(type: "varchar(255)", nullable: true),
                    ImageUrl = table.Column<string>(type: "varchar(255)", nullable: true),
                    HostId = table.Column<int>(type: "INTEGER", nullable: false),
                    WinnerId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameYears", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameYears_Teams_HostId",
                        column: x => x.HostId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GameYears_Teams_WinnerId",
                        column: x => x.WinnerId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GameYears_HostId",
                table: "GameYears",
                column: "HostId");

            migrationBuilder.CreateIndex(
                name: "IX_GameYears_WinnerId",
                table: "GameYears",
                column: "WinnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Games_GameYears_GameYearId",
                table: "Games",
                column: "GameYearId",
                principalTable: "GameYears",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Games_GameYears_GameYearId",
                table: "Games");

            migrationBuilder.DropTable(
                name: "Files");

            migrationBuilder.DropTable(
                name: "GameYears");

            migrationBuilder.RenameColumn(
                name: "GameYearId",
                table: "Games",
                newName: "YearId");

            migrationBuilder.RenameColumn(
                name: "Category",
                table: "Games",
                newName: "Description");

            migrationBuilder.RenameIndex(
                name: "IX_Games_GameYearId",
                table: "Games",
                newName: "IX_Games_YearId");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Games",
                type: "varchar(255)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "Games",
                type: "varchar(255)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Games",
                type: "varchar(255)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Year",
                columns: table => new
                {
                    YearId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    HostId = table.Column<int>(type: "INTEGER", nullable: false),
                    WinnerId = table.Column<int>(type: "INTEGER", nullable: false),
                    Value = table.Column<short>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Year", x => x.YearId);
                    table.ForeignKey(
                        name: "FK_Year_Teams_HostId",
                        column: x => x.HostId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Year_Teams_WinnerId",
                        column: x => x.WinnerId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Year_HostId",
                table: "Year",
                column: "HostId");

            migrationBuilder.CreateIndex(
                name: "IX_Year_WinnerId",
                table: "Year",
                column: "WinnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Games_Year_YearId",
                table: "Games",
                column: "YearId",
                principalTable: "Year",
                principalColumn: "YearId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
