using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kunigi.Data.Migrations
{
    /// <inheritdoc />
    public partial class yearchange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Games_Teams_HostId",
                table: "Games");

            migrationBuilder.DropForeignKey(
                name: "FK_Games_Teams_WinnerId",
                table: "Games");

            migrationBuilder.DropIndex(
                name: "IX_Games_HostId",
                table: "Games");

            migrationBuilder.DropIndex(
                name: "IX_Games_WinnerId",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "HostId",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "Order",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "WinnerId",
                table: "Games");

            migrationBuilder.RenameColumn(
                name: "Year",
                table: "Games",
                newName: "YearId");

            migrationBuilder.CreateTable(
                name: "Year",
                columns: table => new
                {
                    YearId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Value = table.Column<short>(type: "INTEGER", nullable: false),
                    HostId = table.Column<int>(type: "INTEGER", nullable: false),
                    WinnerId = table.Column<int>(type: "INTEGER", nullable: false)
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
                name: "IX_Games_YearId",
                table: "Games",
                column: "YearId");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Games_Year_YearId",
                table: "Games");

            migrationBuilder.DropTable(
                name: "Year");

            migrationBuilder.DropIndex(
                name: "IX_Games_YearId",
                table: "Games");

            migrationBuilder.RenameColumn(
                name: "YearId",
                table: "Games",
                newName: "Year");

            migrationBuilder.AddColumn<int>(
                name: "HostId",
                table: "Games",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<short>(
                name: "Order",
                table: "Games",
                type: "INTEGER",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<int>(
                name: "WinnerId",
                table: "Games",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Games_HostId",
                table: "Games",
                column: "HostId");

            migrationBuilder.CreateIndex(
                name: "IX_Games_WinnerId",
                table: "Games",
                column: "WinnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Games_Teams_HostId",
                table: "Games",
                column: "HostId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Games_Teams_WinnerId",
                table: "Games",
                column: "WinnerId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
